using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaec.Helper.IoC
{
    public interface IJaecContainer
    {
        public void Load(IServiceCollection services);
    }
}
