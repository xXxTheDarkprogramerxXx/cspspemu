﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpUtils;
using CSPspEmu.Hle.Managers;

namespace CSPspEmu.Hle.Modules.registry
{
	unsafe public class sceReg : HleModuleHost
	{
		/// <summary>
		/// Open the registry
		/// </summary>
		/// <param name="RegParam">A filled in ::RegParam structure</param>
		/// <param name="Mode">Open mode (set to 1)</param>
		/// <param name="RegHandle">Pointer to a REGHANDLE to receive the registry handle</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x92E41280, FirmwareVersion = 150)]
		public int sceRegOpenRegistry(RegParam* RegParam, OpenRegistryMode Mode, RegHandle* RegHandle)
		{
			var HleRegistryNode = new HleRegistryNode(RegParam[0].Name);
			*RegHandle = (RegHandle)HleState.HleRegistryManager.RegHandles.Create(HleRegistryNode);
			return 0;
		}

		/// <summary>
		/// Flush the registry to disk
		/// </summary>
		/// <param name="RegHandle">The open registry handle</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x39461B4D, FirmwareVersion = 150)]
		public int sceRegFlushRegistry(RegHandle RegHandle)
		{
			var HleRegistryNode = HleState.HleRegistryManager.RegHandles.Get((int)RegHandle);
			HleRegistryNode.Flush();
			return 0;
		}

		/// <summary>
		/// Close the registry 
		/// </summary>
		/// <param name="RegHandle">The open registry handle</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0xFA8A5739, FirmwareVersion = 150)]
		public int sceRegCloseRegistry(RegHandle RegHandle)
		{
			HleState.HleRegistryManager.RegHandles.Remove((int)RegHandle);
			return 0;
		}

		public enum OpenRegistryMode : uint
		{
			Read = 1,
			Write = 2,
		}

		/// <summary>
		/// Open a registry directory
		/// </summary>
		/// <param name="RegHandle">The open registry handle</param>
		/// <param name="Name">The path to the dir to open (e.g. /CONFIG/SYSTEM)</param>
		/// <param name="Mode">Open mode (can be 1 or 2, probably read or read/write</param>
		/// <param name="RegCategoryHandle">Pointer to a REGHANDLE to receive the registry dir handle</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x1D8A762E, FirmwareVersion = 150)]
		public int sceRegOpenCategory(RegHandle RegHandle, string Name, OpenRegistryMode Mode, RegCategoryHandle* RegCategoryHandle)
		{
			var HleRegistryNode = HleState.HleRegistryManager.RegHandles.Get((int)RegHandle);
			var HleRegistryCategoryNode = new HleRegistryCategoryNode(HleRegistryNode, Name);
			*RegCategoryHandle = (RegCategoryHandle)HleState.HleRegistryManager.RegCategoryHandles.Create(HleRegistryCategoryNode);

			return 0;
		}


		/// <summary>
		/// Flush the registry directory to disk
		/// </summary>
		/// <param name="RegCategoryHandle">The open registry dir handle</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x0D69BF40, FirmwareVersion = 150)]
		public int sceRegFlushCategory(RegCategoryHandle RegCategoryHandle)
		{
			var HleRegistryCategoryNode = HleState.HleRegistryManager.RegCategoryHandles.Get((int)RegCategoryHandle);
			HleRegistryCategoryNode.Flush();
			return 0;
		}

		/// <summary>
		/// Close the registry directory
		/// </summary>
		/// <param name="RegCategoryHandle">The open registry dir handle</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x0CAE832B, FirmwareVersion = 150)]
		public int sceRegCloseCategory(RegCategoryHandle RegCategoryHandle)
		{
			HleState.HleRegistryManager.RegCategoryHandles.Remove((int)RegCategoryHandle);
			return 0;
		}

		/// <summary>
		/// Get a key's information
		/// </summary>
		/// <param name="RegCategoryHandle">The open registry dir handle</param>
		/// <param name="Name">Name of the key</param>
		/// <param name="RegKeyHandle">Pointer to a REGHANDLE to get registry key handle</param>
		/// <param name="Type">Type of the key, on of ::RegKeyTypes</param>
		/// <param name="Size">The size of the key's value in bytes</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0xD4475AA8, FirmwareVersion = 150)]
		public int sceRegGetKeyInfo(RegCategoryHandle RegCategoryHandle, string Name, RegKeyHandle* RegKeyHandle, RegKeyTypes* Type, uint* Size)
		{
			var HleRegistryCategoryNode = HleState.HleRegistryManager.RegCategoryHandles.Get((int)RegCategoryHandle);
			var KeyNode = HleRegistryCategoryNode.GetKeyByName(Name);
			*RegKeyHandle = KeyNode.Id;
			*Type = KeyNode.Type;
			*Size = KeyNode.Size;

			return 0;
		}

		/// <summary>
		/// Set a key's value
		/// </summary>
		/// <param name="RegCategoryHandle">The open registry dir handle</param>
		/// <param name="Name">The key name</param>
		/// <param name="Buffer">Buffer to hold the value</param>
		/// <param name="Size">The size of the buffer</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x17768E14, FirmwareVersion = 150)]
		public int sceRegSetKeyValue(RegCategoryHandle RegCategoryHandle, string Name, void* Buffer, uint Size)
		{
			throw (new NotImplementedException());
		}

		/// <summary>
		/// Get a key's value
		/// </summary>
		/// <param name="RegCategoryHandle">The open registry dir handle</param>
		/// <param name="RegKeyHandle">The open registry key handler (from ::sceRegGetKeyInfo)</param>
		/// <param name="Buffer">Buffer to hold the value</param>
		/// <param name="Size">The size of the buffer</param>
		/// <returns>0 on success, less than 0 on error</returns>
		[HlePspFunction(NID = 0x28A8E98A, FirmwareVersion = 150)]
		public int sceRegGetKeyValue(RegCategoryHandle RegCategoryHandle, RegKeyHandle RegKeyHandle, void* Buffer, uint Size)
		{
			var HleRegistryCategoryNode = HleState.HleRegistryManager.RegCategoryHandles.Get((int)RegCategoryHandle);
			var KeyNode = HleRegistryCategoryNode.GetKeyNodeById(RegKeyHandle);
			KeyNode.Write(Buffer, Size);
			return 0;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	unsafe public struct RegParam
	{
		/// <summary>
		/// Set to 1 only for system
		/// </summary>
		public uint RegType; // 0x0

		/// <summary>
		/// Seemingly never used, set to ::SYSTEM_REGISTRY
		/// </summary>
		public fixed byte NameRaw[256]; // 0x4-0x104

		/// <summary>
		/// Length of the name
		/// </summary>
		public int NameLength; // 0x104
		
		/// <summary>
		/// Unknown, set to 1
		/// </summary>
		public uint Unknown2; // 0x108
		
		/// <summary>
		/// Unknown, set to 1
		/// </summary>
		public uint Unknown3; // 0x10C

		public string Name
		{
			get
			{
				fixed (byte* Name = NameRaw)
				{
					return PointerUtils.PtrToString(Name, NameLength, Encoding.UTF8);
				}
			}
		}
	}

	public struct RegistryEntry
	{

	}


	/*
	class RegistryNode : VFS {
		RegKeyTypes type;
		ubyte[] value;
		VFS[] childs;
		VFS[] implList() { return childs; }

		this(string name) {
			this.type = RegKeyTypes.REG_TYPE_DIR;
			super(name);
		}

		this(string name, uint value) {
			this.type  = RegKeyTypes.REG_TYPE_INT;
			this.value.length = 4;
			*cast(uint *)this.value.ptr = value;
			super(name);
		}

		this(string name, string value) {
			this.type  = RegKeyTypes.REG_TYPE_STR;
			this.value = cast(ubyte[])value;
			super(name);
		}

		VFS implContains(string name, bool create = false) {
			if (create) {
				auto node = new RegistryNode(name);
				this.childs ~= node;
				return node;
			}
			return null;
		}

		uint typeSize() {
			final switch (type) {
				case RegKeyTypes.REG_TYPE_DIR: return 0;
				case RegKeyTypes.REG_TYPE_INT: return 4;
				case RegKeyTypes.REG_TYPE_STR, RegKeyTypes.REG_TYPE_BIN: return value.length;
			}
		}
	
		Stats implStats() {
			Stats stats;
			{
				stats.phyname = this.name;
				stats.mode = 0;
				stats.attr = 0;
				stats.size = this.typeSize;
				stats.type = (this.type == RegKeyTypes.REG_TYPE_DIR) ? Type.Directory : Type.File;
			}
			return stats;
		}

		Stream implOpen(string name, FileMode mode, int attr) {
			return new MemoryStream(value);
		}
	}
	*/
}
