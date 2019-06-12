using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Infrastructure.CosmosDB.Documents;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Util.ToExpression.UnitTest.Helpers;

namespace Util.ToExpression.UnitTest
{
    [TestClass]
    public class UnitTestExpressions
    {
        public interface IFakeDocumentQuery<T> : IDocumentQuery<T>, IOrderedQueryable<T> { }
        private UserModel _userEntityModel;
        private Mock<ICosmosDbClientFactory> _mockCosmosDbClientFactory;
        private ExpressionsLambdaUtilService<UserModel> _iBaseRepository;
        [TestInitialize]
        public void Initialize()
        {
            _mockCosmosDbClientFactory = new Mock<ICosmosDbClientFactory>();
            _iBaseRepository = new ExpressionsLambdaUtilService<UserModel>();
            _userEntityModel = new UserModel
            {
                Id = "201905:r28cbe20b-025f-4f18-a6d9-1f8728d93774",
                NickName = "demo-user",
                FirstName = "demo",
                LastName = "user",
                Age = 35,
                CreatedOn = "20190507",
                ModifiedOn = "20190507",
                UserType = "user",
                LastActivityDate = null,
                IsActive = true,
                Tests = new[] { new UserModel.Test { LanguageCode = "EN" } }
            };
        }

        [TestMethod]
        public async Task Test_Bool_CreateToExpression()
        {
            var dataSource = new List<UserModel> {
                new UserModel { FirstName = "demo"},
                new UserModel { FirstName = "dem2"}
            }.AsQueryable();

            var result = _iBaseRepository.CreateToExpression("true", "IsActive");

            var documentsRepository = DocumentTestCreateDbRepository(dataSource);

            //Act
            var query = await documentsRepository.GetResult(result);

            //Assert
            Assert.IsNotNull(query);

        }
        [TestMethod]
        public async Task Test_String_CreateToExpression()
        {
            var dataSource = new List<UserModel> {
                new UserModel { FirstName = "demo"},
                new UserModel { FirstName = "dem2"}
            }.AsQueryable();

            var result = _iBaseRepository.CreateToExpression("demo", "FirstName");

            var documentsRepository = DocumentTestCreateDbRepository(dataSource);

            //Act
            var query = await documentsRepository.GetResult(result);

            //Assert
            Assert.IsNotNull(query);

        }
        [TestMethod]
        public async Task Test_Int_CreateToExpression()
        {
            var dataSource = new List<UserModel> {
                new UserModel { FirstName = "demo"},
                new UserModel { FirstName = "dem2"}
            }.AsQueryable();

            var result = _iBaseRepository.CreateToExpression("number35", "Age");

            var documentsRepository = DocumentTestCreateDbRepository(dataSource);

            //Act
            var query = await documentsRepository.GetResult(result);

            //Assert
            Assert.IsNotNull(query);

        }
        [TestMethod]
        public async Task Test_ArrayWithNumber_CreateToExpression()
        {
            var dataSource = new List<UserModel> {
                new UserModel { FirstName = "demo"},
                new UserModel { FirstName = "dem2"}
            }.AsQueryable();

            var result = _iBaseRepository.CreateToExpression("number35", "test[0].Age");

            var documentsRepository = DocumentTestCreateDbRepository(dataSource);

            //Act
            var query = await documentsRepository.GetResult(result);

            //Assert
            Assert.IsNotNull(query);

        }
        [TestMethod]
        public async Task Test_ArrayWithBool_CreateToExpression()
        {
            var dataSource = new List<UserModel> {
                new UserModel { FirstName = "demo"},
                new UserModel { FirstName = "dem2"}
            }.AsQueryable();

            var result = _iBaseRepository.CreateToExpression("true", "test[0].IsActive");

            var documentsRepository = DocumentTestCreateDbRepository(dataSource);

            //Act
            var query = await documentsRepository.GetResult(result);

            //Assert
            Assert.IsNotNull(query);

        }
        [TestMethod]
        public async Task Test_ArrayWithString_CreateToExpression()
        {
            var dataSource = new List<UserModel> {
                new UserModel { FirstName = "demo"},
                new UserModel { FirstName = "dem2"}
            }.AsQueryable();

            var result = _iBaseRepository.CreateToExpression("demo", "test[0].name");

            var documentsRepository = DocumentTestCreateDbRepository(dataSource);

            //Act
            var query = await documentsRepository.GetResult(result);

            //Assert
            Assert.IsNotNull(query);

        }

        [TestMethod, ExpectedException(typeof(ArgumentException), "Param search is null.")]
        public void Test_When_is_null_search()
        {
             _iBaseRepository.CreateToExpression(null, "test[0].name");
        }

        private static DocumentTestCreateDbRepository<UserModel> DocumentTestCreateDbRepository(IQueryable<UserModel> dataSource)
        {
            var response = new FeedResponse<UserModel>(dataSource);

            var mockDocumentQuery = new Mock<IFakeDocumentQuery<UserModel>>();

            mockDocumentQuery.SetupSequence(_ => _.HasMoreResults)
                .Returns(true)
                .Returns(false);

            mockDocumentQuery
                .Setup(_ => _.ExecuteNextAsync<UserModel>(It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var provider = new Mock<IQueryProvider>();
            provider
                .Setup(_ => _.CreateQuery<UserModel>(It.IsAny<System.Linq.Expressions.Expression>()))
                .Returns(mockDocumentQuery.Object);

            mockDocumentQuery.As<IQueryable<UserModel>>().Setup(x => x.Provider).Returns(provider.Object);
            mockDocumentQuery.As<IQueryable<UserModel>>().Setup(x => x.Expression).Returns(dataSource.Expression);
            mockDocumentQuery.As<IQueryable<UserModel>>().Setup(x => x.ElementType).Returns(dataSource.ElementType);
            mockDocumentQuery.As<IQueryable<UserModel>>().Setup(x => x.GetEnumerator())
                .Returns(() => dataSource.GetEnumerator());

            var client = new Mock<IDocumentClient>();

            client.Setup(_ => _.CreateDocumentQuery<UserModel>(It.IsAny<Uri>(), It.IsAny<FeedOptions>()))
                .Returns(mockDocumentQuery.Object);

            var documentsRepository = new DocumentTestCreateDbRepository<UserModel>(client.Object, "123", "123");
            return documentsRepository;
        }
    }
}
