using System;
using System.IO;
using UnityEngine;

public static class WavUtility
{
    const int HEADER_SIZE = 44;

    public static AudioClip ToAudioClip(byte[] fileBytes, string name = "wav")
    {
        int channels = BitConverter.ToInt16(fileBytes, 22);
        int sampleRate = BitConverter.ToInt32(fileBytes, 24);
        int sampleCount = (fileBytes.Length - HEADER_SIZE) / 2;

        AudioClip audioClip = AudioClip.Create(name, sampleCount / channels, channels, sampleRate, false);
        float[] data = new float[sampleCount];

        int offset = 0;
        for (int i = HEADER_SIZE; i < fileBytes.Length; i += 2)
        {
            data[offset++] = BitConverter.ToInt16(fileBytes, i) / 32768.0f;
        }

        audioClip.SetData(data, 0);
        return audioClip;
    }

    public static byte[] FromAudioClip(AudioClip audioClip)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            int sampleCount = audioClip.samples * audioClip.channels;
            int sampleRate = audioClip.frequency;
            int channels = audioClip.channels;

            WriteHeader(stream, audioClip, sampleCount, sampleRate, channels);

            float[] data = new float[sampleCount];
            audioClip.GetData(data, 0);

            Int16[] intData = new Int16[data.Length];
            Byte[] bytesData = new Byte[data.Length * 2];

            for (int i = 0; i < data.Length; i++)
            {
                intData[i] = (short)(data[i] * 32767);
                BitConverter.GetBytes(intData[i]).CopyTo(bytesData, i * 2);
            }

            stream.Write(bytesData, 0, bytesData.Length);
            return stream.ToArray();
        }
    }

    private static void WriteHeader(Stream stream, AudioClip audioClip, int sampleCount, int sampleRate, int channels)
    {
        int byteRate = sampleRate * channels * 2;

        stream.Position = 0;
        stream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
        stream.Write(BitConverter.GetBytes((int)(HEADER_SIZE + sampleCount * 2 - 8)), 0, 4);
        stream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);
        stream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
        stream.Write(BitConverter.GetBytes(16), 0, 4);
        stream.Write(BitConverter.GetBytes((short)1), 0, 2);
        stream.Write(BitConverter.GetBytes((short)channels), 0, 2);
        stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
        stream.Write(BitConverter.GetBytes(byteRate), 0, 4);
        stream.Write(BitConverter.GetBytes((short)(channels * 2)), 0, 2);
        stream.Write(BitConverter.GetBytes((short)16), 0, 2);
        stream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
        stream.Write(BitConverter.GetBytes(sampleCount * 2), 0, 4);
    }

    

    public static byte[] FromAudioClipStream(AudioClip clip)
    {
        using (var memoryStream = new MemoryStream())
        {
            WriteHeader(memoryStream, clip);
            ConvertAndWrite(memoryStream, clip);
            WriteHeader(memoryStream, clip); // Rewrite the header now that we know the length

            return memoryStream.ToArray();
        }
    }

    static void ConvertAndWrite(Stream stream, AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        var intData = new short[samples.Length];
        var bytesData = new byte[samples.Length * 2];

        int rescaleFactor = 32767;
        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArray = BitConverter.GetBytes(intData[i]);
            byteArray.CopyTo(bytesData, i * 2);
        }
        stream.Write(bytesData, 0, bytesData.Length);
    }

    static void WriteHeader(Stream stream, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        stream.Seek(0, SeekOrigin.Begin);

        byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        stream.Write(riff, 0, 4);

        byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
        stream.Write(chunkSize, 0, 4);

        byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        stream.Write(wave, 0, 4);

        byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        stream.Write(fmt, 0, 4);

        byte[] subChunk1 = BitConverter.GetBytes(16);
        stream.Write(subChunk1, 0, 4);

        ushort two = 2;
        ushort one = 1;

        byte[] audioFormat = BitConverter.GetBytes(one);
        stream.Write(audioFormat, 0, 2);

        byte[] numChannels = BitConverter.GetBytes(channels);
        stream.Write(numChannels, 0, 2);

        byte[] sampleRate = BitConverter.GetBytes(hz);
        stream.Write(sampleRate, 0, 4);

        byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
        stream.Write(byteRate, 0, 4);

        ushort blockAlign = (ushort)(channels * 2);
        stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        ushort bps = 16;
        byte[] bitsPerSample = BitConverter.GetBytes(bps);
        stream.Write(bitsPerSample, 0, 2);

        byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        stream.Write(datastring, 0, 4);

        byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        stream.Write(subChunk2, 0, 4);
    }















}
