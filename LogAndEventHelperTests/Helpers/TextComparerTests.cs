using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Mew.Helpers.Tests {
    [TestClass]
    public class TextComparerTests {
        [TestMethod]
        [DataRow("CRATE", "TRACE", 0.73)]
        [DataRow("DWAYNE", "DUANE", 0.82)]
        [DataRow("MARTHA", "MARHTA", 0.94)]
        [DataRow("DIXON", "DICKSONX", 0.76)]
        [DataRow("ВО ВРЕМЯ ВОЙНЫ ОСТАЛИСЬ БЕЗ РАБОТЫ?", "Работаем во время военного положения", 0.76)]
        public void JaroSimilarityTest(string s1, string s2, double res) {
            

            var r = TextComparer.JaroSimilarity(s1, s2);
            Debug.WriteLine(r);
            Assert.IsTrue(Math.Abs(r - res) < 0.01);
        }

        [TestMethod]
        [DataRow("CRATE", "TRACE", 0.73)]
        [DataRow("DWAYNE", "DUANE", 0.84)]
        [DataRow("MARTHA", "MARHTA", 0.96)]
        [DataRow("DIXON", "DICKSONX", 0.81)]
        public void JaroWinklerTest(string s1, string s2, double res) {
            var r = TextComparer.JaroWinkler(s1, s2);
            Debug.WriteLine(r);
            Assert.IsTrue(Math.Abs(r - res) < 0.01);
        }

        [TestMethod]
        public void MassiveTest() {
            var ss = new string[] { "01111", "00101", "11111", "00000" };

            var max_similarity = 0.0;
            var res1 = -1; var res2 = -1;
            for (var i = 0; i < ss.Length; i++)
                for (var j = i + 1; j < ss.Length; j++) {
                    var similarity = TextComparer.JaroWinkler(ss[i], ss[j]);
                    if (max_similarity > similarity) continue;
                    max_similarity = similarity;
                    res1 = i;
                    res2 = j;
                }
            Debug.WriteLine($"Most simular are {res1 + 1} and {res2 + 1}");
        }
    }
}