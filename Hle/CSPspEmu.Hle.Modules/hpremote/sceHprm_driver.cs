﻿using CSPspEmu.Hle.Attributes;

namespace CSPspEmu.Hle.Modules.hpremote
{
    [HlePspModule(ModuleFlags = ModuleFlags.KernelMode | ModuleFlags.Flags0x00010011)]
    public class sceHprm_driver : sceHprm
    {
    }
}