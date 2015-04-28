using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using F3.Infrastructure;
using Google.Apis.Services;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using Ninject;

namespace F3.Business
{
    public class GoogleContactBusiness : IContactBusiness
    {
        [Inject]
        public IAccessToken Token { get; set; }
        
        public async Task<IEnumerable<F3.ViewModels.Contact>> GetContacts()
        {
            var rs = new RequestSettings("F3Test", Token.AccessToken);
            rs.AutoPaging = true;
            var cr = new ContactsRequest(rs);

            var f = cr.GetContacts();
            var c = f.Entries.Select(e => new F3.ViewModels.Contact
            {
                Email = e.Emails.Any() ? e.Emails.First().Address : string.Empty,
                FirstName = e.Name.GivenName,
                LastName = e.Name.FamilyName,
                Id = e.Id
            }).ToList();
            return c;
        }

        public async Task<ViewModels.Contact> AddContact(ViewModels.Contact contact)
        {
            var rs = new RequestSettings("F3Test", Token.AccessToken);
            // AutoPaging results in automatic paging in order to retrieve all contacts
            rs.AutoPaging = true;
            var cr = new ContactsRequest(rs);

            var newEntry = new Contact
            {
                Name = new Name()
                {
                    FullName = string.Format("{0} {1}", contact.FirstName, contact.LastName),
                    GivenName = contact.FirstName,
                    FamilyName = contact.LastName,
                }
            };
            // Set the contact's name.
            // Set the contact's e-mail addresses.
            newEntry.Emails.Add(new EMail
            {
                Primary = true,
                Rel = ContactsRelationships.IsHome,
                Address = contact.Email
            });

            // Insert the contact.
            var feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
            var createdEntry = cr.Insert(feedUri, newEntry);
            contact.Id = createdEntry.Id;
            return contact;
        }
    }
}
