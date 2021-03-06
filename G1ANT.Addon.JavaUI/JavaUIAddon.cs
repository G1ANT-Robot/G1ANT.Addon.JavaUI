﻿using G1ANT.Language;

namespace G1ANT.Addon.JavaUI
{
    [Addon(Name = "JavaUI", Tooltip = "Addon which support java application automations")]
    [Copyright(Author = "G1ANT Robot Ltd", Copyright = "G1ANT Robot Ltd", Email = "hi@g1ant.com", Website = "www.g1ant.com")]
    [License(Type = "LGPL", ResourceName = "License.txt")]
    [CommandGroup(Name = "javaui", Tooltip = "Commands allow java applications automation")]
    public class JavaUIAddon : Language.Addon
    {
    }
}
