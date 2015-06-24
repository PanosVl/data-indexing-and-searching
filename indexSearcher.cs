using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Search.Vectorhighlight;

/*
 * This program hits a Lucene index with queries and returns 
 * search results to the user.
 */

    class indexSearcher
    {
        static void Main(string[] args)
        {
            // Directory where index is saved.
            Directory indexDirectory = FSDirectory.Open(@"C:\Users\pvlachos\Desktop\index");

            // Initializing Lucene.
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            
			// Creating IndexReader instance to read the index file.
            IndexReader reader = IndexReader.Open(indexDirectory, false);
            Lucene.Net.Search.IndexSearcher searcher = new Lucene.Net.Search.IndexSearcher(reader);           

            int totalDocuments = reader.NumDocs(); // returns the total number of documents in the index directory

            QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "mainText", analyzer); // where "mainText", is the field(s) I hit with queries
            string searchTerm;
            Console.WriteLine("You have indexed {0} files in your index folder.", totalDocuments);
            Console.WriteLine("\n");
            Highlighter();
            Console.WriteLine("\n");
            Console.WriteLine("Enter the search term:");
            Console.Write(">");
            while((searchTerm = Console.ReadLine()) != String.Empty)
            {
                Search(searchTerm, searcher, parser, indexDirectory, totalDocuments);
                Console.Write(">");               
            }
            searcher.Dispose();
            reader.Dispose();
            indexDirectory.Dispose();
        }

        // ============================= ADDITIONAL METHODS ====================================
            
        // This is a typical search on a Lucene's index file.
        private static void Search(string searchTerm, Lucene.Net.Search.IndexSearcher searcher, QueryParser parser, Directory indexDirectory, int totalDocuments)
        {   
            // Supply conditions
            Query query = parser.Parse(searchTerm);
			
            // Will store the results (hits).
            TopScoreDocCollector collector = TopScoreDocCollector.Create(totalDocuments, true);
            searcher.Search(query, collector);
            ScoreDoc[] hits = collector.TopDocs().ScoreDocs;
            int counter = 0;
			
            // printing out the results
            foreach (ScoreDoc item in hits)
            {
                int docID = item.Doc;
                Document d = searcher.Doc(docID);
                // Call DisplayMessage(d); to display the message.
                DisplayMessage(d, searchTerm);
                counter++;
            }
            if (counter != 0)
            {
                Console.WriteLine("Found {0} messages that match your search term.", counter);
            }
            else
            {
                Console.WriteLine("There were no results matching your search request.\nSorry :(");
            }
            Console.WriteLine("==============================");
        }

        // This method is printing out the message details given the index document.
        // NOTE: The field "mainText" must be stored in indexing level. Same goes for any
		// other field you want to search.        
        private static void DisplayMessage(Document d, string searchTerm)
        {
            // THIS IS USED IN THE DATABASE INDEXic
            //Console.WriteLine("id: " + d.Get("id") + "\n" + "messageBox: " + d.Get("messageBox") + "\n" + "incoming: " + d.Get("incoming") + "\n" + "date: " + d.Get("date") + "\n" + "mainText: " + d.Get("mainText"));

            // THIS IS USED IN MY TEST FILES
            //Console.WriteLine("id: " + d.Get("id") + "\n" + "mainText: " + d.Get("mainText"));
            string text = d.Get("mainText");
            TermQuery query = new TermQuery(new Term("mainText", searchTerm));
            Lucene.Net.Search.Highlight.IScorer scorer = new QueryScorer(query);
            Highlighter highlighter = new Highlighter(scorer);
            System.IO.StringReader reader = new System.IO.StringReader(text);
            TokenStream tokenStream = new SimpleAnalyzer().TokenStream("mainText", reader);
            String[] toBePrinted = highlighter.GetBestFragments(tokenStream, text, 5); // 5 is the maximum number of fragments that gets tested
           foreach (var word in toBePrinted)
            {
                Console.Write(word);
            }
            
            Console.WriteLine("=====================");
            Console.ReadKey();
        }

        // This method takes a search term and a text as a parameter, and displays the text 
        // with the search term in bold.
        public static void RealHighlighter(string searchTerm, string text)
        {
            TermQuery query = new TermQuery(new Term("mainText", searchTerm));
            Lucene.Net.Search.Highlight.IScorer scorer = new QueryScorer(query);
            Highlighter highlighter = new Highlighter(scorer);
            System.IO.StringReader reader = new System.IO.StringReader(text);
            TokenStream tokenStream = new SimpleAnalyzer().TokenStream("mainText", reader);
            String[] toBePrinted = highlighter.GetBestFragments(tokenStream, text, 5); // 5 is the maximum number of fragments that gets tested
            foreach (var word in toBePrinted)
            {
                Console.Write(word);
            }
        }

        // TEST METHOD FOR HIGHLIGHTING.
        public static void Highlighter()
        {
            string textTest = "I am a man that follows hell.";
            TermQuery queryTest = new TermQuery(new Term("", "hell"));
            Lucene.Net.Search.Highlight.IScorer scorer = new QueryScorer(queryTest);
            Highlighter highlighter = new Highlighter(scorer);
            System.IO.StringReader reader = new System.IO.StringReader(textTest);
            TokenStream tokenStream = new SimpleAnalyzer().TokenStream("field", reader);
            String[] toBePrinted = highlighter.GetBestFragments(tokenStream, textTest, 1); // 1 is the maximum number of fragments that gets tested
            foreach (var word in toBePrinted)
            {
                Console.WriteLine(word);                
            }
        }

    }
