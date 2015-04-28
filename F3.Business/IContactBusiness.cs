using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F3.ViewModels;

namespace F3.Business
{
    public interface IContactBusiness
    {
        Task<IEnumerable<F3.ViewModels.Contact>> GetContacts();
        Task<Contact> AddContact(Contact contact);
    }
}
