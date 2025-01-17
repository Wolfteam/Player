namespace Player.Application.Medias;

public class WavFormat
{
    public int SampleRate { get; private set; }

    public short Channels { get; private set; }

    public short BitsPerSample { get; private set; }

    public long Length { get; private set; }

    public float LengthInSeconds
        => Length / (float) (SampleRate * (BitsPerSample / 8) * Channels);

    public static bool IsWav(Stream source, out byte[] header)
    {
        header = new byte[44];
        return source.Read(header, 0, 44) == 44;
    }

    public static WavFormat FromStream(Stream source)
    {
        if (!IsWav(source, out byte[] header))
        {
            throw new ArgumentException("The provided bytes are not a valid wav file since it does not contain a header");
        }

        short channels = (short)(header[22] | header[23] << 8);
        int sampleRate = header[24] | header[25] << 8 | header[26] << 16 | header[27] << 24;
        short bitsPerSample = (short)(header[34] | header[35] << 8);
        long length = source.Length - 44;

        return new WavFormat
        {
            Channels = channels,
            SampleRate = sampleRate,
            BitsPerSample = bitsPerSample,
            Length = length
        };
    }

    public static WavFormat FromBytes(byte[] bytes)
    {
        using MemoryStream source = new MemoryStream(bytes);
        return FromStream(source);
    }
}