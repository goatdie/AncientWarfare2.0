using Figurebox.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figurebox.utils.extensions;
public static class CityExtension
{
    public static AW_City AW(this City pCity) => pCity as AW_City;
}
