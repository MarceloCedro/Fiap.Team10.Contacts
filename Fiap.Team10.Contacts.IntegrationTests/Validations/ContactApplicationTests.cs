using Fiap.Team10.Contacts.Application.Services;
using Fiap.Team10.Contacts.Domain.Interfaces.Applications;
using Fiap.Team10.Contacts.Domain.Interfaces.Repositories;
using Fiap.Team10.Contacts.Domain.Interfaces.Services;
using Fiap.Team10.Contacts.Domain.Services;
using Fiap.Team10.Contacts.Infrastructure.Repositories;
using Fiap.Team10.Contacts.IntegrationTests.Config;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.Team10.Contacts.IntegrationTests.Validations
{
    public class ContactApplicationTests : BaseServiceTests
    {
        private readonly IContactService _contactService;
        private readonly IContactApplication _contactApplicationException;
        private readonly IContactApplication _contactApplication;
        private readonly IContactRepository _contactRepository;
        private Mock<ILogger<ContactService>> _loggerServiceMock;
        private Mock<ILogger<ContactApplication>> _loggerApplicationMock;
        public readonly Random RandomId;

        public ContactApplicationTests()
        {
            _contactRepository = new ContactRepository(_context);
            _loggerServiceMock = new Mock<ILogger<ContactService>>();
            _loggerApplicationMock = new Mock<ILogger<ContactApplication>>();
            _contactService = new ContactService(_contactRepository, _loggerServiceMock.Object);
            _contactApplication = new ContactApplication(_contactService, _loggerApplicationMock.Object);
            _contactApplicationException = new ContactApplication(null, _loggerApplicationMock.Object);
            RandomId = new Random();
        }

        [Fact]
        public async Task InsertContactSuccessAsync()
        {
            var contact = ContactFixtures.CreateFakeContactCreateDto();
            var insertResult = await _contactApplication.AddContactAsync(contact);
            Assert.NotNull(insertResult);
            Assert.Equal(insertResult.Success, true);
            Assert.Equal(insertResult.Message, "Contato inserido com sucesso.");
        }

        [Fact]
        public async Task InsertContactExceptionAsync()
        {
            var contact = ContactFixtures.CreateFakeContactCreateDto();
            var insertResult = await _contactApplicationException.AddContactAsync(contact);
            Assert.NotNull(insertResult);
            Assert.Equal(insertResult.Success, false);
            Assert.Equal(insertResult.Message, "Ocorreu um problema ao tentar inserir o registro.");
        }

        [Fact]
        public async Task UpdateContactNotFoundAsync()
        {
            var contactFake = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));
            var contact = ContactFixtures.CreateFakeContactUpdateDto(contactFake);
            var updateResult = await _contactApplication.UpdateContactAsync(contact);
            Assert.NotNull(updateResult);
            Assert.Equal(updateResult.Success, false);
            Assert.Equal(updateResult.Message, "Contato não encontrado");
        }

        [Fact]
        public async Task UpdateContactSuccessAsync()
        {
            var contact = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));

            await _context.AddRangeAsync(contact);

            await SaveChanges();
            var contactUpdate = ContactFixtures.CreateFakeContactUpdateDto(contact);
            var updateResult = await _contactApplication.UpdateContactAsync(contactUpdate);
            Assert.NotNull(updateResult);
            Assert.Equal(updateResult.Success, true);
            Assert.Equal(updateResult.Message, "Contato alterado com sucesso.");
        }

        [Fact]
        public async Task UpdateContactExceptionAsync()
        {
            var contact = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));

            await _context.AddRangeAsync(contact);

            await SaveChanges();
            var contactUpdate = ContactFixtures.CreateFakeContactUpdateDto(contact);
            var updateResult = await _contactApplicationException.UpdateContactAsync(contactUpdate);
            Assert.NotNull(updateResult);
            Assert.Equal(updateResult.Success, false);
            Assert.Equal(updateResult.Message, "Ocorreu um problema ao tentar atualizar o registro.");
        }

        [Fact]
        public async Task GetAllContactSuccessAsync()
        {
            var contact1 = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));
            var contact2 = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));
            var contact3 = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));

            await _context.AddRangeAsync(contact1, contact2, contact3);

            await SaveChanges();

            var contactList = await _contactApplication.GetAllContactsAsync();
            Assert.NotNull(contactList);
            Assert.NotEmpty(contactList);
        }

        [Fact]
        public async Task GetAllContactExceptionAsync()
        {
            var contact1 = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));
            var contact2 = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));
            var contact3 = ContactFixtures.CreateFakeContact(RandomId.Next(999999999));

            await _context.AddRangeAsync(contact1, contact2, contact3);

            await SaveChanges();

            var contactList = await _contactApplicationException.GetAllContactsAsync();
            Assert.Null(contactList);
        }
    }
}
