using System;
using System.Collections.Generic;
using System.Text;

namespace FourPrime.Application.Abstractions.Repositories
{
    public interface IReloadable
    {
        Task ReloadAsync();
    }
}
