using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Digipolis.Paging.Predicates;

namespace Digipolis.Paging.UnitTests.Predicates
{
    public class OrderPredicatesTests
    {
        public OrderPredicatesTests()
        {

        }


        [Fact]
        public void OrderBy_Without_Property_Name_Does_Not_Order_If_No_Class_Properties_Exist()
        {
            var queryable = new List<OrderClassWithoutProperties>
            {
                new OrderClassWithoutProperties(),
                new OrderClassWithoutProperties(),
                new OrderClassWithoutProperties(),
                new OrderClassWithoutProperties(),
                new OrderClassWithoutProperties(),
                new OrderClassWithoutProperties()
            }.AsQueryable();

            var result = queryable.OrderBy();

            Assert.Equal(queryable.First(), result.First());
            Assert.Equal(queryable.Last(), result.Last());
        }

        [Fact]
        public void OrderBy_Without_Property_Name_Orders_On_First_Property_If_Id_Property_Does_Not_Exist()
        {
            var queryable = new List<OrderClassWithoutId>
            {
                new OrderClassWithoutId(5),
                new OrderClassWithoutId(7),
                new OrderClassWithoutId(1),
                new OrderClassWithoutId(10),
                new OrderClassWithoutId(4),
                new OrderClassWithoutId(6)
            }.AsQueryable();

            var result = queryable.OrderBy();

            var expectedFirstResult = queryable.OrderBy(x => x.A).First();
            var expectedLastResult = queryable.OrderBy(x => x.A).Last();

            Assert.Equal(expectedFirstResult, result.First());
            Assert.Equal(expectedFirstResult.A, result.First().A);
            Assert.Equal(expectedFirstResult.B, result.First().B);
            Assert.Equal(expectedFirstResult.C, result.First().C);

            Assert.Equal(expectedLastResult, result.Last());
            Assert.Equal(expectedLastResult.A, result.Last().A);
            Assert.Equal(expectedLastResult.B, result.Last().B);
            Assert.Equal(expectedLastResult.C, result.Last().C);
        }

        [Fact]
        public void OrderBy_Without_Property_Name_Results_In_Order_By_Id_If_Id_Property_Exists()
        {
            var queryable = new List<OrderClass>
            {
                new OrderClass(5),
                new OrderClass(7),
                new OrderClass(1),
                new OrderClass(10),
                new OrderClass(4),
                new OrderClass(6)
            }.AsQueryable();

            var result = queryable.OrderBy();

            Assert.Equal(1, result.First().Id);
            Assert.Equal(10, result.Last().Id);
        }

        [Fact]
        public void OrderBy_With_Property_Name_Results_In_Order_By_Property_If_Property_Exists()
        {
            var queryable = new List<OrderClass>
            {
                new OrderClass(5),
                new OrderClass(7),
                new OrderClass(1),
                new OrderClass(10),
                new OrderClass(4),
                new OrderClass(6)
            }.AsQueryable();

            var result = queryable.OrderBy("B");

            var expectedFirstResult = queryable.OrderBy(x => x.B).First();
            var expectedLastResult = queryable.OrderBy(x => x.B).Last();

            Assert.Equal(expectedFirstResult, result.First());
            Assert.Equal(expectedLastResult, result.Last());
        }

        [Fact]
        public void OrderBy_With_Minus_Property_Name_Results_In_Order_By_Descending_Property_If_Property_Exists()
        {
            var queryable = new List<OrderClass>
            {
                new OrderClass(5),
                new OrderClass(7),
                new OrderClass(1),
                new OrderClass(10),
                new OrderClass(4),
                new OrderClass(6)
            }.AsQueryable();

            var result = queryable.OrderBy("-B");

            var expectedFirstResult = queryable.OrderByDescending(x => x.B).First();
            var expectedLastResult = queryable.OrderByDescending(x => x.B).Last();

            Assert.Equal(expectedFirstResult, result.First());
            Assert.Equal(expectedLastResult, result.Last());
        }

        [Fact]
        public void OrderBy_With_Property_Name_Results_In_ArgumentException_If_Property_Does_Not_Exist()
        {
            var queryable = new List<OrderClass>
            {
                new OrderClass(5),
                new OrderClass(7),
                new OrderClass(1),
                new OrderClass(10),
                new OrderClass(4),
                new OrderClass(6)
            }.AsQueryable();

            Assert.Throws<ArgumentException>(() => queryable.OrderBy("E"));
        }

        [Fact]
        public void OrderBy_With_Minus_Property_Name_Results_In_ArgumentException_If_Property_Does_Not_Exist()
        {
            var queryable = new List<OrderClass>
            {
                new OrderClass(5),
                new OrderClass(7),
                new OrderClass(1),
                new OrderClass(10),
                new OrderClass(4),
                new OrderClass(6)
            }.AsQueryable();

            Assert.Throws<ArgumentException>(() => queryable.OrderBy("-E"));
        }

        [Fact]
        public void OrderBy_With_Multiple_Property_Names_Results_In_Order_By_Then_By_If_Properties_Exists()
        {
            var queryable = new List<OrderClass>
            {
                new OrderClass(5)
                {
                    Id = 1
                },
                new OrderClass(7),
                new OrderClass(1),
                new OrderClass(10),
                new OrderClass(4),
                new OrderClass(6)
            }.AsQueryable();

            var result = queryable.OrderBy("Id,A");

            var expectedFirstResult = queryable.First(x => x.Id == 1 && x.A == "b");
            var expectedSecondResult = queryable.First(x => x.Id == 1 && x.A == "f");
            var expectedLastResult = queryable.OrderBy(x => x.Id).Last();

            Assert.Equal(expectedFirstResult, result.First());
            Assert.Equal(expectedSecondResult, result.Skip(1).Take(1).First());
            Assert.Equal(expectedLastResult, result.Last());


            result = queryable.OrderBy("-Id,A");

            expectedFirstResult = queryable.OrderByDescending(x => x.Id).First();
            var expectedFifthResult = queryable.First(x => x.Id == 1 && x.A == "b");
            expectedLastResult = queryable.First(x => x.Id == 1 && x.A == "f");

            Assert.Equal(expectedFirstResult, result.First());
            Assert.Equal(expectedFifthResult, result.Skip(4).Take(1).First());
            Assert.Equal(expectedLastResult, result.Last());


            result = queryable.OrderBy("Id,-A");

            expectedFirstResult = queryable.First(x => x.Id == 1 && x.A == "f");
            expectedSecondResult = queryable.First(x => x.Id == 1 && x.A == "b");
            expectedLastResult = queryable.OrderBy(x => x.Id).Last();

            Assert.Equal(expectedFirstResult, result.First());
            Assert.Equal(expectedSecondResult, result.Skip(1).Take(1).First());
            Assert.Equal(expectedLastResult, result.Last());
        }


        private class OrderClassWithoutProperties
        {
            public OrderClassWithoutProperties()
            {
            }
        }

        private class OrderClassWithoutId
        {
            public OrderClassWithoutId(int index)
            {
                A = ((char)('a' + index)).ToString();
                B = ((char)('Z' - index)).ToString();
                C = DateTime.Today.AddDays(index);
            }
            public string A { get; set; }

            public string B { get; set; }

            public DateTime? C { get; set; }
        }

        private class OrderClass : OrderClassWithoutId
        {
            public OrderClass(int index) : base(index)
            {
                Id = index;
            }
            public int Id { get; set; }

        }
    }
}
