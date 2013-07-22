﻿//#define ENABLE_PITCH

using System;
using CSPspEmu.Hle.Attributes;
using CSPspEmu.Core.Audio;
using CSharpUtils;

namespace CSPspEmu.Hle.Modules.sc_sascore
{
	[HlePspModule(ModuleFlags = ModuleFlags.UserMode | ModuleFlags.Flags0x00010011)]
	public unsafe partial class sceSasCore : HleModuleHost
	{
		static Logger Logger = Logger.GetLogger("sceSasCore");

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SasCorePointer"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0xBD11B7C2, FirmwareVersion = 150)]
		public int __sceSasGetGrain(uint SasCorePointer)
		{
			var SasCore = GetSasCore(SasCorePointer);
			return SasCore.GrainSamples;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SasCorePointer"></param>
		/// <param name="Grain"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0xD1E0A01E, FirmwareVersion = 150)]
		public int __sceSasSetGrain(uint SasCorePointer, int Grain)
		{
			var SasCore = GetSasCore(SasCorePointer);
			try
			{
				return 0;
				//return SasCore.GrainSamples;
			}
			finally
			{
				SasCore.GrainSamples = Grain;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SasCorePointer"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0xE175EF66, FirmwareVersion = 150)]
		public OutputMode __sceSasGetOutputmode(uint SasCorePointer)
		{
			var SasCore = GetSasCore(SasCorePointer);

			//throw(new NotImplementedException());
			return SasCore.OutputMode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SasCorePointer"></param>
		/// <param name="OutputMode"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0xE855BF76, FirmwareVersion = 150)]
		public int __sceSasSetOutputmode(uint SasCorePointer, OutputMode OutputMode)
		{
			var SasCore = GetSasCore(SasCorePointer);

			//throw(new NotImplementedException());
			try
			{
				return 0;
				//return SasCore.OutputMode;
			}
			finally
			{
				SasCore.OutputMode = OutputMode;
			}
		}

		protected int[] VoiceOnCount;
		protected StereoIntSoundSample[] BufferTemp;
		protected StereoShortSoundSample[] BufferShort;
		protected StereoShortSoundSample[] MixBufferShort;


		/// <summary>
		/// Initialized a sasCore structure.
		/// </summary>
		/// <remarks>
		/// PSP can only handle one at a time.
		/// </remarks>
		/// <example>
		/// __sceSasInit(&sasCore, PSP_SAS_GRAIN_SAMPLES, PSP_SAS_VOICES_MAX, OutputMode.PSP_SAS_OUTPUTMODE_STEREO, 44100);
		/// </example>
		/// <param name="SasCorePointer">Pointer to a <see cref="SasCore"/> structure that will contain information.</param>
		/// <param name="GrainSamples">Number of grainSamples</param>
		/// <param name="MaxVoices">Max number of voices</param>
		/// <param name="OutputMode">Out Mode</param>
		/// <param name="SampleRate">Sample Rate</param>
		/// <returns>0 on success</returns>
		[HlePspFunction(NID = 0x42778A9F, FirmwareVersion = 150)]
		//[HlePspNotImplemented]
		public uint __sceSasInit(uint SasCorePointer, int GrainSamples, int MaxVoices, OutputMode OutputMode, int SampleRate)
		{
			if (SampleRate != 44100)
			{
				throw (new SceKernelException(SceKernelErrors.ERROR_SAS_INVALID_SAMPLE_RATE));
			}

			if (GrainSamples < 0x40 || GrainSamples > 0x800 || (GrainSamples & 0x1F) != 0)
			{
				throw (new SceKernelException(SceKernelErrors.ERROR_SAS_INVALID_GRAIN));
			}

			if (MaxVoices < 1 || MaxVoices > PSP_SAS_VOICES_MAX)
			{
				throw(new SceKernelException(SceKernelErrors.ERROR_SAS_INVALID_MAX_VOICES));
			}

			if (OutputMode != sc_sascore.OutputMode.PSP_SAS_OUTPUTMODE_STEREO && OutputMode != sc_sascore.OutputMode.PSP_SAS_OUTPUTMODE_MULTICHANNEL)
			{
				throw (new SceKernelException(SceKernelErrors.ERROR_SAS_INVALID_OUTPUT_MODE));
			}

			var SasCore = GetSasCore(SasCorePointer, CreateIfNotExists: true);
			{
				SasCore.Initialized = true;
				SasCore.GrainSamples = GrainSamples;
				SasCore.MaxVoices = MaxVoices;
				SasCore.OutputMode = OutputMode;
				SasCore.SampleRate = SampleRate;
			}

			VoiceOnCount = new int[SasCore.GrainSamples * 2];
			BufferTemp = new StereoIntSoundSample[SasCore.GrainSamples * 2];
			BufferShort = new StereoShortSoundSample[SasCore.GrainSamples * 2];
			MixBufferShort = new StereoShortSoundSample[SasCore.GrainSamples * 2];

			return 0;
		}

		/// <summary>
		/// Return a bitfield indicating the end of the voices.
		/// </summary>
		/// <param name="SasCorePointer">Core</param>
		/// <returns>A set of flags indiciating the end of the voices.</returns>
		[HlePspFunction(NID = 0x68A46B95, FirmwareVersion = 150)]
		public uint __sceSasGetEndFlag(uint SasCorePointer)
		{
			var SasCore = GetSasCore(SasCorePointer);
			return SasCore.EndFlags;
		}

		/// <summary>
		/// Sets the <see cref="WaveformEffectType"/> to the specified <see cref="SasCore"/>.
		/// </summary>
		/// <param name="SasCorePointer">Core</param>
		/// <param name="WaveformEffectType">Effect</param>
		/// <returns>0 on success.</returns>
		[HlePspFunction(NID = 0x33D4AB37, FirmwareVersion = 150)]
		public uint __sceSasRevType(uint SasCorePointer, WaveformEffectType WaveformEffectType)
		{
			var SasCore = GetSasCore(SasCorePointer);
			SasCore.WaveformEffectType = WaveformEffectType;
			return 0;
		}

		/// <summary>
		/// Sets the WaveformEffectIsDry and WaveformEffectIsWet to the specified SasCore.
		/// </summary>
		/// <param name="SasCorePointer">SasCore</param>
		/// <param name="WaveformEffectIsDry">WaveformEffectIsDry</param>
		/// <param name="WaveformEffectIsWet">WaveformEffectIsWet</param>
		/// <returns>0 on success.</returns>
		[HlePspFunction(NID = 0xF983B186, FirmwareVersion = 150)]
		public uint __sceSasRevVON(uint SasCorePointer, bool WaveformEffectIsDry, bool WaveformEffectIsWet)
		{
			var SasCore = GetSasCore(SasCorePointer);
			SasCore.WaveformEffectIsDry = WaveformEffectIsDry;
			SasCore.WaveformEffectIsWet = WaveformEffectIsWet;
			return 0;
		}

		/// <summary>
		/// Sets the effect left and right volumes for the specified SasCore.
		/// </summary>
		/// <param name="SasCorePointer">SasCore</param>
		/// <param name="LeftVolume">Left volume</param>
		/// <param name="RightVolume">Right volume</param>
		/// <returns>0 on success</returns>
		[HlePspFunction(NID = 0xD5A229C9, FirmwareVersion = 150)]
		public uint __sceSasRevEVOL(uint SasCorePointer, int LeftVolume, int RightVolume)
		{
			var SasCore = GetSasCore(SasCorePointer);
			SasCore.LeftVolume = LeftVolume;
			SasCore.RightVolume = RightVolume;
			//throw(new NotImplementedException());
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SasCorePointer"></param>
		/// <param name="Delay"></param>
		/// <param name="Feedback"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0x267A6DD2, FirmwareVersion = 150)]
		public int __sceSasRevParam(uint SasCorePointer, int Delay, int Feedback)
		{
			var SasCore = GetSasCore(SasCorePointer);

			SasCore.Delay = Delay;
			SasCore.Feedback = Feedback;

			//throw(new NotImplementedException());
			return 0;
		}

		/// <summary>
		/// Pauses a set of voice channels for that SasCore.
		/// </summary>
		/// <param name="SasCorePointer">SasCore</param>
		/// <param name="voice_bits">Voice Bit Set</param>
		/// <returns>0 on success.</returns>
		[HlePspFunction(NID = 0x787D04D5, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int __sceSasSetPause(uint SasCorePointer, uint voice_bits)
		{
			//throw(new NotImplementedException());
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SasCorePointer"></param>
		/// <returns></returns>
		[HlePspFunction(NID = 0x2C8E6AB3, FirmwareVersion = 150)]
		//[HlePspNotImplemented]
		public int __sceSasGetPauseFlag(uint SasCorePointer)
		{
			//throw(new NotImplementedException());
			return 0;
		}

		/// <summary>
		/// Process the voices and generate the next samples.
		/// Mix the resulting samples in an exiting buffer.
		/// </summary>
		/// <param name="SasCorePointer">SasCore handle</param>
		/// <param name="SasInOut">
		///		address for the input and output buffer.
		///		Samples are stored as 2 16-bit values
		///		(left then right channel samples)
		/// </param>
		/// <param name="LeftVolume">Left channel volume, [0..0x1000].</param>
		/// <param name="RightVolume">Right channel volume, [0..0x1000].</param>
		/// <returns>
		///		if OK 0
		///		ERROR_SAS_NOT_INIT if an invalid SasCore handle is provided
		/// </returns>
		[HlePspFunction(NID = 0x50A14DFC, FirmwareVersion = 150)]
		//[HlePspNotImplemented]
		public int __sceSasCoreWithMix(uint SasCorePointer, short* SasInOut, int LeftVolume, int RightVolume)
		{
			return __sceSasCore_Internal(GetSasCore(SasCorePointer), SasInOut, SasInOut, LeftVolume, RightVolume);
		}


		/// <summary>
		/// Process the voices and generate the next samples.
		/// </summary>
		/// <param name="SasCorePointer">SasCore handle</param>
		/// <param name="SasOut">
		///		address for the output buffer.
		///		Samples are stored as 2 16-bit values
		///		(left then right channel samples)
		/// </param>
		/// <returns>
		///		if OK 0
		///		ERROR_SAS_NOT_INIT if an invalid sasCore handle is provided
		/// </returns>
		[HlePspFunction(NID = 0xA3589D81, FirmwareVersion = 150)]
		//[HlePspNotImplemented]
		public int __sceSasCore(uint SasCorePointer, short* SasOut)
		{
			return __sceSasCore_Internal(GetSasCore(SasCorePointer), SasOut, null, 0x1000, 0x1000);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SasCore"></param>
		/// <param name="SasOut"></param>
		/// <param name="SasIn"></param>
		/// <param name="LeftVolume"></param>
		/// <param name="RightVolume"></param>
		/// <returns></returns>
		public int __sceSasCore_Internal(SasCore SasCore, short* SasOut, short* SasIn, int LeftVolume, int RightVolume)
		{
			fixed (StereoShortSoundSample* BufferShortPtr = BufferShort)
			fixed (StereoIntSoundSample* BufferTempPtr = BufferTemp)
			fixed (int* VoiceOnCountPtr = VoiceOnCount)
			{
				if (SasCore.OutputMode != OutputMode.PSP_SAS_OUTPUTMODE_STEREO)
				{
					Logger.Unimplemented("SasCore.OutputMode != OutputMode.PSP_SAS_OUTPUTMODE_STEREO");
				}

				int NumberOfChannels = SasCore.OutputMode == OutputMode.PSP_SAS_OUTPUTMODE_STEREO ? 2 : 1;
				int NumberOfSamples = SasCore.GrainSamples * 2 / NumberOfChannels;

				for (int n = 0; n < NumberOfSamples; n++)
				{
					BufferTempPtr[n] = default(StereoIntSoundSample);
					VoiceOnCountPtr[n] = 0;
				}

				// Read and mix voices.
				foreach (var Voice in SasCore.Voices)
				{
					if (Voice.OnAndPlaying)
					{
						for (int n = 0, Pos = 0; n < NumberOfSamples; n++, Pos += Voice.Pitch)
						{
							if (Voice.SampleOffset < Voice.Vag.SamplesCount)
							{
#if !ENABLE_PITCH
								int PosDiv = n;
#else
								int PosDiv = Pos / PSP_SAS_PITCH_BASE;
#endif

								VoiceOnCountPtr[PosDiv]++;
								BufferTempPtr[PosDiv] += Voice.Vag.GetSampleAt(Voice.SampleOffset++);
							}
							else
							{
								Voice.SetPlaying(false);
								break;
							}
						}
					}
				}

				// Normalize output
				for (int n = 0; n < NumberOfSamples; n++)
				{
					if (VoiceOnCount[n] > 0)
					{
						BufferShortPtr[n] = (BufferTempPtr[n] / VoiceOnCount[n]);
					}
					else
					{
						BufferShortPtr[n] = default(StereoShortSoundSample);
					}
				}

				// Output converted 44100 data
				if (NumberOfChannels == 1)
				{
					for (int n = 0; n < NumberOfSamples; n++)
					{
						SasOut[n] = (short)((int)BufferShortPtr[n].Left * LeftVolume >> 12);
					}
				}
				else
				{
					for (int n = 0; n < NumberOfSamples; n++)
					{
						SasOut[n * 2 + 0] = (short)((int)BufferShortPtr[n].Left * LeftVolume >> 12);
						SasOut[n * 2 + 1] = (short)((int)BufferShortPtr[n].Right * RightVolume >> 12);
					}
				}
			}

			//throw(new NotImplementedException());
			return 0;
		}

		/// <summary>
		/// Get the current envelope height for all the voices.
		/// </summary>
		/// <param name="SasCorePointer">SasCore handle</param>
		/// <param name="Heights">
		/// (int *) address where to return the envelope heights,
		/// stored as 32 bit values [0..0x40000000].
		///     heightsAddr[0] = envelope height of voice 0
		///     heightsAddr[1] = envelope height of voice 1
		///     ...
		/// </param>
		/// <returns>
		/// 0 if OK
		/// ERROR_SAS_NOT_INIT if an invalid SasCore handle is provided
		/// </returns>
		[HlePspFunction(NID = 0x07F58C24, FirmwareVersion = 150)]
		public int __sceSasGetAllEnvelopeHeights(uint SasCorePointer, int* Heights)
		{
			var SasCore = GetSasCore(SasCorePointer);
			foreach (var Voice in SasCore.Voices)
			{
				Voice.EnvelopeHeight = *Heights++;
			}
			return 0;
		}

		[HlePspFunction(NID = 0xE1CD9561, FirmwareVersion = 500)]
		[HlePspNotImplemented]
		public int __sceSasSetVoicePCM()
		{
			return 0;
		}

		[HlePspFunction(NID = 0xD5EBBBCD, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int __sceSasSetSteepWave()
		{
			return 0;
		}

		[HlePspFunction(NID = 0xA232CBE6, FirmwareVersion = 150)]
		[HlePspNotImplemented]
		public int __sceSasSetTriangularWave()
		{
			return 0;
		}
	}
}