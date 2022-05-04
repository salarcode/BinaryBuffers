namespace BinaryBuffers
{
    using System;
    using System.IO;

    internal static class ThrowHelper
    {
        public static void ThrowPositionLessThanZeroException(string positionParameterName)
        {
            ThrowPositionLessThanZeroException(positionParameterName, "Position (zero-based)");
        }

        public static void ThrowOffsetLessThanZeroException(string positionParameterName)
        {
            ThrowPositionLessThanZeroException(positionParameterName, "Offset (zero-based)");
        }

        public static void ThrowLengthLessThanZeroException(string positionParameterName)
        {
            ThrowPositionLessThanZeroException(positionParameterName, "Length");
        }


        private static void ThrowPositionLessThanZeroException(string positionParameterName, string positionWord)
        {
            throw new ArgumentOutOfRangeException(positionParameterName, $"{positionWord} must be greater than or equal to zero.");
        }



        public static void ThrowPositionGreaterThanLengthOfByteArrayException(string positionParameterName)
        {
            ThrowPositionGreaterThanDataStreamLengthException(positionParameterName, "Position (zero-based)", "byte array");
        }
        public static void ThrowPositionGreaterThanLengthOfReadOnlyMemoryException(string positionParameterName)
        {
            ThrowPositionGreaterThanDataStreamLengthException(positionParameterName, "Position (zero-based)", "read-only memory");
        }

        public static void ThrowOffsetGreaterThanLengthOfByteArrayException(string offsetParameterName)
        {
            ThrowPositionGreaterThanDataStreamLengthException(offsetParameterName, "Offset (zero-based)", "byte array");
        }
        public static void ThrowOffsetGreaterThanLengthOfReadOnlyMemoryException(string offsetParameterName)
        {
            ThrowPositionGreaterThanDataStreamLengthException(offsetParameterName, "Offset (zero-based)", "read-only memory");
        }

        public static void ThrowLengthGreaterThanLengthOfByteArrayException(string offsetParameterName)
        {
            ThrowPositionGreaterThanDataStreamLengthException(offsetParameterName, "Length", "byte array");
        }
        public static void ThrowLengthGreaterThanLengthOfReadOnlyMemoryException(string offsetParameterName)
        {
            ThrowPositionGreaterThanDataStreamLengthException(offsetParameterName, "Length", "read-only memory");
        }



        private static void ThrowPositionGreaterThanDataStreamLengthException(string positionParameterName, string positionWord, string dataStreamType)
        {
            throw new ArgumentOutOfRangeException(positionParameterName, $"{positionWord} must be less than the size of the underlying {dataStreamType}.");
        }


        public static void ThrowOffsetPlusLengthGreaterThanLengthOfByteArrayException()
        {
            throw new ArgumentException("Offset plus length must be less than the size of the underlying byte array.");
        }
        
        public static void ThrowEndOfDataException()
        {
            throw new EndOfStreamException("Reached to end of data");
        }

        public static IOException ThrowDecimalReadingException(ArgumentException argumentException)
        {
            return new IOException("Failed to read decimal value", argumentException);
        }
    }
}
