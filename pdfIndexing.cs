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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

/**
 * This program indexes .pdf files into a Lucene document.
 **/
    class pdfIndexing
    {
        static void Main(string[] args)
        {
            // Path to index file.
            Directory pdfIndex = FSDirectory.Open(@"/* PASTE THE PATH WHERE YOUR INDEX WILL BE SAVED */");
            
			// Creating analyzer to make index searchable.
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            // Creating IndexWriter
            IndexWriter.MaxFieldLength mfl = new IndexWriter.MaxFieldLength(100000);
            IndexWriter writer = new IndexWriter(pdfIndex, analyzer, mfl);

            // Full path to input .pdf files.  
            string[] filesList = System.IO.Directory.GetFiles(@"/* PASTE THE PATH TO YOUR INPUT FILE(S) */", "*.pdf");
            
			/* INDEX FIELDS:
			** id & body are the fields to my Lucene Index, 
			** you can change those fields accordingly to your 
			** needs
			*/
			int idNumber = 0;
            string pdfText;
			
            foreach (string file in filesList)
            {
                pdfText = GetAllText(file);
                AddToIndex(idNumber, pdfText, writer);
                idNumber++;
            }
            writer.Dispose();
            DisplayAllInput(filesList);
            Console.ReadKey();
        }

		// ========================== ADDITIONAL METHODS ==========================
		
        // This method uses iTextSharp library to extract the whole text from a PDF.
        public static string GetAllText(String pdfPath)
        {
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(pdfPath);
            System.IO.StringWriter output = new System.IO.StringWriter();
            for (int i = 1; i <= reader.NumberOfPages; i++)
                output.WriteLine(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));                
            return output.ToString();
        }


        // This method indexes the given text.
        private static void AddToIndex(int id, string text, IndexWriter writer)
        {
            Term term = new Term("id", id.ToString());
            Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
            doc.Add(new Field("id", id.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("mainText", text, Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
            writer.UpdateDocument(term, doc);
        }

        // This method displays all files in input directory.
        private static void DisplayAllInput(string[] files)
        {
            int index = 1;
            foreach (string file in files)
            {
                Console.WriteLine("Text body of file {0}: {1}", index, System.IO.File.ReadAllText(file));
                index++;
                Console.ReadKey();
            }
            index--;
            Console.WriteLine("Ok, done!The total number of files in your input directory, is {0}", index);
        } 
    }