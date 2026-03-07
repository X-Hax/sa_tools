using System;
using System.Collections.Generic;
using VGAudio.Formats;
using VGAudio.Formats.Pcm16;

namespace SADXsndSharp
{
	// The line below prevents Windows Forms Designer from opening on this .cs file
	[System.ComponentModel.DesignerCategory("")] partial class FormViewBlocker { }
	public partial class Form1
	{
		/// <summary>
		/// Converts a byte array containing an ADX file to a byte array containing a PCM WAV file.
		/// </summary>
		/// <param name="adx"></param>
		public static byte[] AdxToWav(byte[] adx)
		{
			// TODO: loop points
			var reader = new VGAudio.Containers.Adx.AdxReader();
			// ReadWithConfig returns both the IAudioFormat and the container-specific config
			var audioWithConfig = reader.ReadWithConfig(adx);

			IAudioFormat format = audioWithConfig.AudioFormat;
			// Convert to PCM16
			Pcm16Format pcm = format.ToPcm16();
			// pcm.Channels is short[][]; pcm.SampleRate is an int

			// WAV converter
			int channels = pcm.ChannelCount;
			int sampleRate = pcm.SampleRate;
			int sampleCount = pcm.SampleCount;

			// Interleave channels
			List<byte> result = new List<byte>();
			int bytesPerSample = 2;
			int blockAlign = channels * bytesPerSample;
			int dataSize = sampleCount * blockAlign;

			// RIFF header
			result.AddRange(System.Text.Encoding.ASCII.GetBytes("RIFF"));
			result.AddRange(BitConverter.GetBytes((int)(36 + dataSize)));
			result.AddRange(System.Text.Encoding.ASCII.GetBytes("WAVE"));

			// fmt chunk
			result.AddRange(System.Text.Encoding.ASCII.GetBytes("fmt "));
			result.AddRange(BitConverter.GetBytes((int)16));
			result.AddRange(BitConverter.GetBytes((short)1)); // PCM
			result.AddRange(BitConverter.GetBytes((short)channels));
			result.AddRange(BitConverter.GetBytes(sampleRate));
			result.AddRange(BitConverter.GetBytes(sampleRate * blockAlign));
			result.AddRange(BitConverter.GetBytes((short)blockAlign));
			result.AddRange(BitConverter.GetBytes((short)(bytesPerSample * 8)));

			// data chunk
			result.AddRange(System.Text.Encoding.ASCII.GetBytes("data"));
			result.AddRange(BitConverter.GetBytes(dataSize));

			// Write interleaved samples
			for (int i = 0; i < sampleCount; i++)
			{
				for (int ch = 0; ch < channels; ch++)
				{
					short sample = pcm.Channels[ch][i];
					result.AddRange(BitConverter.GetBytes(sample));
				}
			}
			return result.ToArray();
		}
	}
}