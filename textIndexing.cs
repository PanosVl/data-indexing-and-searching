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

/**
 * This program indexes .txt files into a Lucene document.
 **/

    class textIndexing
    {
        static void Main(string[] args)
        {
            // Path to index file.
            Directory indexDirectory = FSDirectory.Open(@"/* PASTE THE PATH WHERE YOUR INDEX WILL BE SAVED */");

            // Creating Analyzer to make index searchable.
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            
            // Creating IndexWriter
            IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(100000);
            IndexWriter writer = new IndexWriter(indexDirectory, analyzer, mfl);

            // Full path to input .txt files.
            string[] filesList = System.IO.Directory.GetFiles(@"/* PASTE THE PATH TO YOUR INPUT FILE(S) */", "*.txt");
            
			/* INDEX FIELDS:
			** id & body are the fields to my Lucene Index, 
			** you can change those fields accordingly to your 
			** needs
			*/
			int idNumber = 0;
            string body;
			
            foreach (string file in filesList)
            {
                body = System.IO.File.ReadAllText(file);
                AddToIndex(idNumber, body, writer);
                idNumber++;
             }
            writer.Dispose();
            
        }

        // ========================== ADDITIONAL METHODS ==========================
		
		/*
		** This method does the most of the work: it adds the content bellow the 
		** the given fields to our index file.
		*/
        private static void AddToIndex(int id, string text, IndexWriter writer)
        {
            Term term = new Term("id", id.ToString());
            Document doc = new Document();            
            doc.Add(new Field("id", id.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("mainText", text, Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
            writer.UpdateDocument(term, doc);
        }

        // This method displays all files in input directory
        private static void DisplayAllInput(string[] files)
        {
            int index = 1;
            foreach(string file in files)
            {
                Console.WriteLine("Text body of file {0}: {1}", index, System.IO.File.ReadAllText(file));
                index++;
                Console.ReadKey();
            }
            index--;
            Console.WriteLine("Ok, done!The total number of files in your input directory, is {0}", index);
        }        
    }
