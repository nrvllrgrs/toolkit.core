using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace ToolkitEditor
{
	public static class AudioUtil
	{
		private const uint HEADER_SIZE = 44;
		private const float RESCALE_FACTOR = 32767; // to convert float to Int16

		public static void Save(string filepath, AudioClip clip, bool trim = false)
		{
			// Make sure directory exists if user is saving to sub dir.
			Directory.CreateDirectory(Path.GetDirectoryName(filepath));

			using (var fileStream = new FileStream(filepath, FileMode.Create))
			using (var writer = new BinaryWriter(fileStream))
			{
				var wav = GetWav(clip, out var length, trim);
				writer.Write(wav, 0, (int)length);
			}
		}

		public static byte[] GetWav(AudioClip clip, out uint length, bool trim = false)
		{
			var data = ConvertAndWrite(clip, out length, out var samples, trim);
			WriteHeader(data, clip, length, samples);

			return data;
		}

		private static byte[] ConvertAndWrite(AudioClip clip, out uint length, out uint samplesAfterTrimming, bool trim)
		{
			var samples = new float[clip.samples * clip.channels];

			clip.GetData(samples, 0);

			var sampleCount = samples.Length;
			var start = 0;
			var end = sampleCount - 1;

			if (trim)
			{
				for (var i = 0; i < sampleCount; i++)
				{
					if ((short)(samples[i] * RESCALE_FACTOR) == 0)
						continue;

					start = i;
					break;
				}

				for (var i = sampleCount - 1; i >= 0; i--)
				{
					if ((short)(samples[i] * RESCALE_FACTOR) == 0)
						continue;

					end = i;
					break;
				}
			}

			var buffer = new byte[(sampleCount * 2) + HEADER_SIZE];

			var p = HEADER_SIZE;
			for (var i = start; i <= end; i++)
			{
				var value = (short)(samples[i] * RESCALE_FACTOR);
				buffer[p++] = (byte)(value >> 0);
				buffer[p++] = (byte)(value >> 8);
			}

			length = p;
			samplesAfterTrimming = (uint)(end - start + 1);
			return buffer;
		}

		private static void AddDataToBuffer(byte[] buffer, ref uint offset, byte[] addBytes)
		{
			foreach (var b in addBytes)
			{
				buffer[offset++] = b;
			}
		}

		private static void WriteHeader(byte[] stream, AudioClip clip, uint length, uint samples)
		{
			var hz = (uint)clip.frequency;
			var channels = (ushort)clip.channels;

			var offset = 0u;

			var riff = Encoding.UTF8.GetBytes("RIFF");
			AddDataToBuffer(stream, ref offset, riff);

			var chunkSize = BitConverter.GetBytes(length - 8);
			AddDataToBuffer(stream, ref offset, chunkSize);

			var wave = Encoding.UTF8.GetBytes("WAVE");
			AddDataToBuffer(stream, ref offset, wave);

			var fmt = Encoding.UTF8.GetBytes("fmt ");
			AddDataToBuffer(stream, ref offset, fmt);

			var subChunk1 = BitConverter.GetBytes(16u);
			AddDataToBuffer(stream, ref offset, subChunk1);

			const ushort two = 2;
			const ushort one = 1;

			var audioFormat = BitConverter.GetBytes(one);
			AddDataToBuffer(stream, ref offset, audioFormat);

			var numChannels = BitConverter.GetBytes(channels);
			AddDataToBuffer(stream, ref offset, numChannels);

			var sampleRate = BitConverter.GetBytes(hz);
			AddDataToBuffer(stream, ref offset, sampleRate);

			var byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
			AddDataToBuffer(stream, ref offset, byteRate);

			var blockAlign = (ushort)(channels * 2);
			AddDataToBuffer(stream, ref offset, BitConverter.GetBytes(blockAlign));

			ushort bps = 16;
			var bitsPerSample = BitConverter.GetBytes(bps);
			AddDataToBuffer(stream, ref offset, bitsPerSample);

			var dataString = Encoding.UTF8.GetBytes("data");
			AddDataToBuffer(stream, ref offset, dataString);

			var subChunk2 = BitConverter.GetBytes(samples * 2);
			AddDataToBuffer(stream, ref offset, subChunk2);
		}
	}
}