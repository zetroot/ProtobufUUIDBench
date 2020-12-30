using BenchmarkDotNet.Attributes;
using Google.Protobuf;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufUUIDBench
{
    [MemoryDiagnoser]
    public class StrBinBench
    {
        private Guid src;
        
        [GlobalSetup]
        public void Setup()
        {
            src = Guid.NewGuid();            
        }


        [Benchmark]
        public Guid StringUUID()
        {            
            var protoUUID = new UUIDstring { StrValue = src.ToString() };
            var bytes = protoUUID.ToByteArray();
            var contructed = new UUIDstring();
            contructed.MergeFrom(bytes);
            return new Guid(contructed.StrValue);                
        } 

        [Benchmark]
        public Guid SpanBinaryUUID()
        {
            Span<byte> guidBuf = stackalloc byte[16];
            Span<byte> serializedDataMemorySrc = stackalloc byte[32];
            if (!src.TryWriteBytes(guidBuf)) throw new Exception();
            var protoUUID = new UUIDbin { BinValue = ByteString.CopyFrom(guidBuf) };
            Span<byte> bytes = serializedDataMemorySrc.Slice(0, protoUUID.CalculateSize());
            //var bytes = protoUUID.ToByteArray();
            protoUUID.WriteTo(bytes);
            var contructed = new UUIDbin();
            contructed.MergeFrom(ByteString.CopyFrom(bytes));
            return new Guid(contructed.BinValue.Span);            
        }

        [Benchmark]
        public Guid ArrBinaryUUID()
        {            
            var protoUUID = new UUIDbin { BinValue = ByteString.CopyFrom(src.ToByteArray()) };            
            var bytes = protoUUID.ToByteArray();            
            var contructed = new UUIDbin();
            contructed.MergeFrom(bytes);
            return new Guid(contructed.BinValue.ToByteArray());            
        }
    }
}
