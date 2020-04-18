using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using System;
using System.IO;

namespace Benchmarks.IO
{
    public class MultiRuntimeConfig : ManualConfig
    {
        public MultiRuntimeConfig()
        {
            // Use a short config because we want to check a trend rather than absolute accuracy for now while investigating
            // performance difference between the stream and wrapper implementations.
            this.AddJob(
                Job.Default.WithRuntime(ClrRuntime.Net472).WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(3),
                Job.Default.WithRuntime(CoreRuntime.Core31).WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(3),
                Job.Default.WithRuntime(CoreRuntime.Core21).WithLaunchCount(1).WithWarmupCount(3).WithIterationCount(3));
        }
    }

    [Config(typeof(MultiRuntimeConfig))]
    public class BufferedStreams
    {
        private readonly byte[] buffer = CreateTestBytes();
        private readonly byte[] chunk1 = new byte[2];
        private readonly byte[] chunk2 = new byte[2];

        private MemoryStream stream1;
        private MemoryStream stream2;
        private MemoryStream stream3;
        private MemoryStream stream4;
        private MemoryStream stream5;
        private MemoryStream stream6;
        private BufferedReadStream bufferedStream1;
        private BufferedReadStream bufferedStream2;
        private BufferedReadStreamWrapper bufferedStreamWrap1;
        private BufferedReadStreamWrapper bufferedStreamWrap2;

        [GlobalSetup]
        public void CreateStreams()
        {
            this.stream1 = new MemoryStream(this.buffer);
            this.stream2 = new MemoryStream(this.buffer);
            this.stream3 = new MemoryStream(this.buffer);
            this.stream4 = new MemoryStream(this.buffer);
            this.stream5 = new MemoryStream(this.buffer);
            this.stream6 = new MemoryStream(this.buffer);
            this.bufferedStream1 = new BufferedReadStream(this.stream3);
            this.bufferedStream2 = new BufferedReadStream(this.stream4);
            this.bufferedStreamWrap1 = new BufferedReadStreamWrapper(this.stream5);
            this.bufferedStreamWrap2 = new BufferedReadStreamWrapper(this.stream6);
        }

        [GlobalCleanup]
        public void DestroyStreams()
        {
            this.bufferedStream1?.Dispose();
            this.bufferedStream2?.Dispose();
            this.bufferedStreamWrap1?.Dispose();
            this.bufferedStreamWrap2?.Dispose();
            this.stream1?.Dispose();
            this.stream2?.Dispose();
            this.stream3?.Dispose();
            this.stream4?.Dispose();
            this.stream5?.Dispose();
            this.stream6?.Dispose();
        }

        [Benchmark]
        public int StandardStreamRead()
        {
            int r = 0;
            Stream stream = this.stream1;
            byte[] b = this.chunk1;

            for (int i = 0; i < stream.Length / 2; i++)
            {
                r += stream.Read(b, 0, 2);
            }

            return r;
        }

        [Benchmark]
        public int BufferedReadStreamRead()
        {
            int r = 0;
            BufferedReadStream reader = this.bufferedStream1;
            byte[] b = this.chunk2;

            for (int i = 0; i < reader.Length / 2; i++)
            {
                r += reader.Read(b, 0, 2);
            }

            return r;
        }

        [Benchmark]
        public int BufferedReadStreamWrapRead()
        {
            int r = 0;
            BufferedReadStreamWrapper reader = this.bufferedStreamWrap1;
            byte[] b = this.chunk2;

            for (int i = 0; i < reader.Length / 2; i++)
            {
                r += reader.Read(b, 0, 2);
            }

            return r;
        }

        [Benchmark(Baseline = true)]
        public int StandardStreamReadByte()
        {
            int r = 0;
            Stream stream = this.stream2;

            for (int i = 0; i < stream.Length; i++)
            {
                r += stream.ReadByte();
            }

            return r;
        }

        [Benchmark]
        public int BufferedReadStreamReadByte()
        {
            int r = 0;
            BufferedReadStream reader = this.bufferedStream2;

            for (int i = 0; i < reader.Length; i++)
            {
                r += reader.ReadByte();
            }

            return r;
        }

        [Benchmark]
        public int BufferedReadStreamWrapReadByte()
        {
            int r = 0;
            BufferedReadStreamWrapper reader = this.bufferedStreamWrap2;

            for (int i = 0; i < reader.Length; i++)
            {
                r += reader.ReadByte();
            }

            return r;
        }

        [Benchmark]
        public int ArrayReadByte()
        {
            byte[] b = this.buffer;
            int r = 0;
            for (int i = 0; i < b.Length; i++)
            {
                r += b[i];
            }

            return r;
        }

        private static byte[] CreateTestBytes()
        {
            var buffer = new byte[BufferedReadStream.BufferLength * 3];
            var random = new Random();
            random.NextBytes(buffer);

            return buffer;
        }
    }
}
