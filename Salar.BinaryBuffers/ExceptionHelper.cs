using System;
using System.IO;

namespace Salar.BinaryBuffers;

internal static class ExceptionHelper
{
	public static ArgumentOutOfRangeException PositionLessThanZeroException(string positionParameterName)
	{
		return PositionLessThanZeroException(positionParameterName, "Position (zero-based)");
	}

	public static ArgumentOutOfRangeException OffsetLessThanZeroException(string positionParameterName)
	{
		return PositionLessThanZeroException(positionParameterName, "Offset (zero-based)");
	}

	public static ArgumentOutOfRangeException LengthLessThanZeroException(string positionParameterName)
	{
		return PositionLessThanZeroException(positionParameterName, "Length");
	}


	private static ArgumentOutOfRangeException PositionLessThanZeroException(string positionParameterName, string positionWord)
	{
		return new ArgumentOutOfRangeException(positionParameterName, $"{positionWord} must be greater than or equal to zero.");
	}



	public static ArgumentOutOfRangeException PositionGreaterThanLengthOfByteArrayException(string positionParameterName)
	{
		return PositionGreaterThanDataStreamLengthException(positionParameterName, "Position (zero-based)", "byte array");
	}

	public static ArgumentOutOfRangeException PositionGreaterThanLengthOfReadOnlyMemoryException(string positionParameterName)
	{
		return PositionGreaterThanDataStreamLengthException(positionParameterName, "Position (zero-based)", "read-only memory");
	}

	private static ArgumentOutOfRangeException PositionGreaterThanDataStreamLengthException(string positionParameterName, string positionWord, string dataStreamType)
	{
		return new ArgumentOutOfRangeException(positionParameterName, $"{positionWord} must be equal to or less than the size of the underlying {dataStreamType}.");
	}


	public static ArgumentException LengthGreaterThanEffectiveLengthOfByteArrayException()
	{
		return new ArgumentException("Offset plus length must be less than the size of the underlying byte array.");
	}

	public static EndOfStreamException EndOfDataException()
	{
		return new EndOfStreamException("Reached to end of data");
	}

	public static IOException DecimalReadingException(ArgumentException argumentException)
	{
		return new IOException("Failed to read decimal value", argumentException);
	}

	internal static ObjectDisposedException DisposedException(string? objectName)
	{
		return new ObjectDisposedException(objectName);
	}
}
