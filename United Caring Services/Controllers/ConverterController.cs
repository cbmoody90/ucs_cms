using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace United_Caring_Services.Controllers
{
    public class ConverterController : Controller
    {
        public string convertToHTML(string page, string text, string images, string urls, string urlnames, string files, string filenames)
        {
            bool docs = text.Contains("@docinsert");
            bool imgContain = text.Contains("@imginsert");
            bool breaks = text.Contains("@break");
            bool url = text.Contains("@url");
            bool list = text.Contains("@list");
            bool listu = text.Contains("@ulist");
            bool slides = text.Contains("@slides");
            string newText = text;
            var myStringBuilder = new StringBuilder(text);


            /*
             * Order on this is key. Doing the list items and carousel first allow for the image, url, file changes to occur after the precursor html 
             * has been set. This eliminates the guess work needed to define the length to remove the short code and insert the updated html.
            */
            if (list)
            {
                //replace @list with <ul> with bullets
                myStringBuilder = ListCheck(text,myStringBuilder);

            }
            if (listu)
            {
                //replace @ulist with an <ul> with list-style = none;
                myStringBuilder = UnOrderedListCheck(text, myStringBuilder);
            }

            if (slides)
            {
                // replace the @slides(x;y;z;) with a bootstrap carousel predefined for height of 600px;
                myStringBuilder = SlideCheck(text, myStringBuilder, page);

            }
            if (imgContain)
            {
                //replace @imginsert with <img>
                myStringBuilder = ImageCheck(text,myStringBuilder, images, page);
            }
            if (docs)
            {
                //check if we have any files that need anchor tags
                myStringBuilder = FileCheck(text,myStringBuilder, files, filenames, page);
            }
            if (url)
            {
                //check for urls needed to be changed to anchor tags
                myStringBuilder = AnchorCheck(text, myStringBuilder, urls, urlnames);
            }
            if (breaks)
            {
                //replace @break with <br /> tags.
                myStringBuilder = BreakCheck(text, myStringBuilder);
            }

            newText = myStringBuilder.ToString();


            return newText;
        }

        public StringBuilder ListCheck(string text,StringBuilder myStringBuilder)
        {
            int count = Regex.Matches(text, "@list").Count;
            for (int d = 0; d < count; d++)
            {
                int listStart = text.IndexOf("@list(");
                int listEnd = text.IndexOf(")@", listStart);
                int length = listEnd - (listStart + 6);
                string listString = myStringBuilder.ToString(listStart + 6, length);
                string[] listItems = listString.Split(';');
                Array.Reverse(listItems);
                int totalRemove = listEnd + 2 - listStart;
                myStringBuilder.Remove(listStart, totalRemove);
                for (int i = 0; i < listItems.Length; i++)
                {
                    if (i == listItems.Length - 1)
                    {
                        myStringBuilder.Insert(listStart, "<ul><li>" + listItems[i] + "</li>");
                    }
                    else if (i == 0)
                    {
                        myStringBuilder.Insert(listStart, "<li>" + listItems[i] + "</li></ul>");
                    }
                    else
                    {
                        myStringBuilder.Insert(listStart, "<li>" + listItems[i] + "</li>");
                    }
                }
            }
            return myStringBuilder;
        }

        public StringBuilder UnOrderedListCheck(string text, StringBuilder myStringBuilder)
        {
            int count = Regex.Matches(text, "@ulist").Count;
            for (int d = 0; d < count; d++)
            {
                int listStart = text.IndexOf("@ulist(");
                int listEnd = text.IndexOf(")@", listStart);
                int length = listEnd - (listStart + 7);
                string listString = myStringBuilder.ToString(listStart + 7, length);
                string[] listItems = listString.Split(';');
                Array.Reverse(listItems);
                int totalRemove = listEnd + 2 - listStart;
                myStringBuilder.Remove(listStart, totalRemove);
                for (int i = 0; i < listItems.Length; i++)
                {
                    if (i == listItems.Length - 1)
                    {
                        myStringBuilder.Insert(listStart, "<ul style='list-style: none'><li>" + listItems[i] + "</li>");
                    }
                    else if (i == 0)
                    {
                        myStringBuilder.Insert(listStart, "<li>" + listItems[i] + "</li></ul>");
                    }
                    else
                    {
                        myStringBuilder.Insert(listStart, "<li>" + listItems[i] + "</li>");
                    }
                }
            }
            return myStringBuilder;
        }

        public StringBuilder SlideCheck(string text, StringBuilder myStringBuilder, String page)
        {
            int count = Regex.Matches(text, "@slides").Count;
            for (int d = 0; d < count; d++)
            {
                int listStart = text.IndexOf("@slides(");
                int listEnd = text.IndexOf(")@", listStart);
                int length = listEnd - (listStart + 8);
                string listString = myStringBuilder.ToString(listStart + 8, length);
                string[] listItems = listString.Split(';');
                //Make sure to reverse or the order it inserts the html code will be backwards.
                Array.Reverse(listItems);
                int totalRemove = listEnd + 2 - listStart;
                myStringBuilder.Remove(listStart, totalRemove);
                for (int i = 0; i < listItems.Length; i++)
                {
                    if (i == listItems.Length - 1)
                    {
                        myStringBuilder.Insert(listStart, "<div id='" + page + "Carousel' class='carousel slide' style='max-width:600px' data-ride='carousel'><div class='carousel-inner'><div class='item active'><img class='d-block w-100' src='\\Assets\\" + page + "\\" + listItems[i] + "' alt='Slide" + i + "'/> </div>");
                    }
                    else if (i == 0)
                    {
                        myStringBuilder.Insert(listStart, "<div class='item'><img class='d-block w-100' src='\\Assets\\" + page + "\\" + listItems[i] + "' alt='Slide" + i + "'/></div></div><a class='left carousel-control' href='#" + page + "Carousel' role='button' data-slide='prev'><span class='glyphicon glyphicon-chevron-left' aria-hidden='true'></span><span class='sr-only'>Previous</span></a><a class='right carousel-control' href='#" + page + "Carousel' role='button' data-slide='next'><span class='glyphicon glyphicon-chevron-right' aria-hidden='true'></span><span class='sr-only'>Next</span></a></div>");
                    }
                    else
                    {
                        myStringBuilder.Insert(listStart, "<div class='item'><img class='d-block w-100' src='\\Assets\\" + page + "\\" + listItems[i] + "' alt='Slide" + i + "'/> </div>");
                    }
                }
            }

            return myStringBuilder;
        }

        public StringBuilder ImageCheck(String text,StringBuilder myStringBuilder,string images,string page)
        {
            if (images != null)
            {
                string[] values = images.Split(',');

                string image = "@imginsert";
                int count = Regex.Matches(text, image).Count;
                for (int i = 0; i < count; i++)
                {
                    int replacements = myStringBuilder.ToString().IndexOf("@imginsert");
                    myStringBuilder.Remove(replacements, 10);
                    myStringBuilder.Insert(replacements, "<img class=\"img-responsive\" src=\"\\Assets\\" + page + "\\" + values[i] + " \"/>");
                }
            }

            return myStringBuilder;
        }

        public StringBuilder FileCheck(String text,StringBuilder myStringBuilder,string files, string filenames,string page)
        {
            if (files != null)
            {
                string[] fileList = files.Split(',');
                if (filenames != null)
                {
                    string[] fileNameList = filenames.Split(',');
                    string fileCode = "@docinsert";
                    int count = Regex.Matches(text, fileCode).Count;
                    for (int i = 0; i < count; i++)
                    {
                        int replacements = myStringBuilder.ToString().IndexOf("@docinsert");
                        myStringBuilder.Remove(replacements, 10);
                        myStringBuilder.Insert(replacements, "<a href='\\Assets\\" + page + "\\" + fileList[i] + "'>" + fileNameList[i] + "</a>");
                    }
                }
            }

            return myStringBuilder;
        }

        public StringBuilder AnchorCheck(string text,StringBuilder myStringBuilder, string urls, string urlnames)
        {
            if (urls != null)
            {
                string[] urlVals = urls.Split(',');
                if (urlnames != null)
                {
                    string[] urlNames = urlnames.Split(',');
                    string urlCheck = "@url";
                    int count = Regex.Matches(text, urlCheck).Count;
                    for (int i = 0; i < count; i++)
                    {
                        int replacements = myStringBuilder.ToString().IndexOf("@url");
                        myStringBuilder.Remove(replacements, 4);
                        myStringBuilder.Insert(replacements, "<a href='" + urlVals[i] + "'>" + urlNames[i] + "</a>");
                    }
                }
            }

            return myStringBuilder;
        }

        public StringBuilder BreakCheck(string text, StringBuilder myStringBuilder)
        {
            string breaks = "@break";
            int count = Regex.Matches(text, breaks).Count;
            for (int i = 0; i < count; i++)
            {
                int replacements = myStringBuilder.ToString().IndexOf("@break");
                myStringBuilder.Remove(replacements, 6);
                myStringBuilder.Insert(replacements, "<br />");
            }

            return myStringBuilder;
        }

        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public void PostEdit(string text, string images, string urls, string urlnames, string files, string filenames, string page, string id)
        {
            ConverterController converter = new ConverterController();
            string htmlFormatted = converter.convertToHTML(page, text, images, urls, urlnames, files, filenames);
            EditContent(page, id, htmlFormatted);

        }

        [Authorize]
        [ValidateInput(false)]
        public Boolean EditContent(string page, string id, string content)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"exec insert_content @page = @pages, @id = @id, @content = @content";
                    cmd.Parameters.Add(new SqlParameter("@pages", SqlDbType.NVarChar) { Value = page });
                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar) { Value = id });
                    cmd.Parameters.Add(new SqlParameter("@content", SqlDbType.NVarChar) { Value = content });
                    cmd.ExecuteNonQuery();
                    connection.Close();

                    return true;

                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
