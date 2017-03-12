/*
	Header setting file Virtual Machine FlameVM
*/
#include <Engine.vm>;
#include <Core.vm>;

#harmony true;

#byteset 0x021 >> 0x00200AF100;
#byteset 0x451 >> 0x00200FF200;
#byteset 0x011 >> 0x00200CF300;

<"AllowDebuggerStatement">("false");
<"Culture">("en-US");
<"Debug">("true");
<"DiscardGlobal">("false");
<"LimitRecursion">("25");
<"Strict">("true");

#include <ext.lib>();