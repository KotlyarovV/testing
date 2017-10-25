using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Challenge
{
	[TestFixture]
	public class WordsStatistics_Tests
	{
		public static string Authors = "Котляров"; // "Egorov Shagalina"

		public virtual IWordsStatistics CreateStatistics()
		{
			// меняется на разные реализации при запуске exe
			return new WordsStatistics();
		}

		private IWordsStatistics statistics;

		[SetUp]
		public void SetUp()
		{
			statistics = CreateStatistics();
		}

		[Test]
		public void GetStatistics_IsEmpty_AfterCreation()
		{
			statistics.GetStatistics().Should().BeEmpty();
		}

        
		[Test]
		public void GetStatistics_ContainsItem_AfterAddition()
		{
			statistics.AddWord("abc");
			statistics.GetStatistics().Should().Equal(Tuple.Create(1, "abc"));
		}
        

            /*
	    [TestCase("123", ExpectedResult = 123, TestName = "integer")]
        public Tuple<int, string> GetStatistics_ContainsItem_AfterAddition(string str)
	    {
	        statistics.AddWord(str);
	        statistics.GetStatistics().Should().Equal(Tuple.Create(1, "abc"));
	    }
        */
        [Test]
		public void GetStatistics_ContainsManyItems_AfterAdditionOfDifferentWords()
		{
			statistics.AddWord("abc");
			statistics.AddWord("def");
			statistics.GetStatistics().Should().HaveCount(2);
		}

	    [Test]
	    public void GetStatistics_ContainsManyItems_AfterAdditionLongUpperString()
	    {
	        statistics.AddWord("QWERtyuiopasdfghj");
	        statistics.GetStatistics().Should().Equal(Tuple.Create(1, "qwertyuiop")); ;
	    }

	    [Test]
	    public void GetStatistics_ContainsManyItems_FiveTenLengthString()
	    {
	        statistics.AddWord("qwertyui");
	        statistics.GetStatistics().Should().Equal(Tuple.Create(1, "qwertyui")); ;
	    }

	    [Test]
	    public void GetStatistics_ContainsManyItems_WordB()
	    {
	        statistics.AddWord("Б");
	        statistics.GetStatistics().Should().Equal(Tuple.Create(1, "б")); ;
	    }
        
        [Test]
        public void GetStatistics_ManyTimes()
        {
            statistics.AddWord("qwertyui");
            statistics.GetStatistics();
            statistics.AddWord("qwertyui");
            statistics.GetStatistics().Should().Equal(Tuple.Create(2, "qwertyui")); 
        }
        

        [Test]
	    public void GetStatistics_ContainsManyItems_ElevenLengthString()
	    {
	        statistics.AddWord("qwertyuiopa");
	        statistics.GetStatistics().Should().Equal(Tuple.Create(1, "qwertyuiop" +
	                                                                  "")); ;
	    }

	    [Test]
	    public void GetStatistics_ContainsManyItems_ElevenWhiteStringLengthString()
	    {
	        statistics.AddWord("          qwertyuiopa");
	        statistics.GetStatistics().Should().Equal(Tuple.Create(1, "          " +
	                                                                  "")); ;
	    }

	    [Test]
	    public void GetStatistics_TwoStatistcs()
	    {
	        statistics.AddWord("qwertyuiopa");
	        IWordsStatistics st = CreateStatistics();
	        statistics.GetStatistics().Should().HaveCount(1);
	    }

        [Test]
	    public void GetStatistics_ContainsManyItems_EmptyString()
	    {
	        statistics.AddWord("");
	        statistics.GetStatistics().Should().HaveCount(0); ;
	    }

        [Test]
	    public void GetStatistics_ContainsManyItems_AfterWhiteSpace()
	    {
	        statistics.AddWord("   ");
	        statistics.GetStatistics().Should().HaveCount(0); ;
	    }


	    [Test, Timeout(300)]
	    public void GetStatistics_ALotOfStrings()
	    {
            for (var i = 0; i < 13000; i++)
	        statistics.AddWord(i.ToString());
	        statistics.GetStatistics().Should().HaveCount(13000); ;
	    }

	    [Test, Timeout(400)]
	    public void GetStatistics_ALotOfSameStrings()
	    {
	        for (var i = 0; i < 13000; i++)
	        {
	            statistics.AddWord(string.Format("asd {0}", i));
	            statistics.AddWord(string.Format("asd {0}", i));

	        }
	        statistics.GetStatistics().Should().HaveCount(13000); ;
	    }

        [Test]
	    public void GetStatistics_ContainsManyItems_CheckRightOrder()
	    {
	        
	        statistics.AddWord("ac");
	        statistics.AddWord("ac");
	        statistics.AddWord("af");
            statistics.AddWord("ab");
	        statistics.AddWord("aB");
	        statistics.GetStatistics().Should()
	            .Equal(new[]
	            {
	                new Tuple<int, string>(2, "ab"), new Tuple<int, string>(2, "ac"),
	                new Tuple<int, string>(1, "af")
                });
	    }

	    [Test]
	    public void GetStatistics_ContainsManyItems_CheckRightOrderSame()
	    {

	        statistics.AddWord("ac");
	        statistics.AddWord("ac");
	        statistics.AddWord("ac");
            statistics.AddWord("ab");
	        statistics.AddWord("aB");
	        statistics.GetStatistics().Should()
	            .Equal(new[]
	            {
	                new Tuple<int, string>(3, "ac"), new Tuple<int, string>(2, "ab")
	            });
	    }


        [Test]
	    public void GetStatistics_CheckArgumentException()
	    {
	        Assert.Throws<ArgumentNullException>(() => statistics.AddWord(null)); ;
	    }
        
	    [Test]
	    public void GetStatistics_CheckOrder()
	    {
	        statistics.AddWord("abc");
	        statistics.AddWord("abc");
	        statistics.AddWord("ab");
            statistics.GetStatistics().Should().HaveCount(2); ;
	    }


        [Test]
	    public void GetStatistics_ContainsItem_AfterAdditionSameElements()
	    {
	        statistics.AddWord("abc");
	        statistics.AddWord("abc");
            statistics.GetStatistics().Should().Equal(Tuple.Create(2, "abc"));
	    }

        // Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki
    }
}