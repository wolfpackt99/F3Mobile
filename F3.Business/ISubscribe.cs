using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F3.ViewModels;

namespace F3.Business
{
    public interface ISubscribe
    {
        Task<bool> Add(Contact contact);
    }
}
