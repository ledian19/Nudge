using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Nudge.Utilities {
    public class AppUtils {

        public static string getConString() {
            return ConfigurationManager.ConnectionStrings["NudgeConnectionString"].ConnectionString;
        }

        #region Categories

        public static List<category> getCategories(int user_id) {
            var myCategories = new List<category>();
            try {
                using (var con = new SqlConnection(getConString())) {
                    con.Open();
                    using (var cmd = con.CreateCommand()) {
                        cmd.CommandText = @"WITH final_categories As 
                                            ( 
                                                SELECT		c.category_id, c.parent_category, c.category_name, 0 AS depth, CAST('' AS VARCHAR(255)) AS path
                                                FROM		Categories c
                                                WHERE		parent_category IS NULL AND c.user_id=@uid

                                                UNION All 
                                                SELECT		c2.category_id, c2.parent_category, c2.category_name, depth+1, CAST(f.path + '/ ' + c2.category_name AS VARCHAR(255)) AS path
                                                FROM		Categories c2 
	                                            JOIN		final_categories f  ON f.category_id = c2.parent_category 
                                            ) 
                                            SELECT		category_id, category_name, parent_category, depth, path
                                            FROM		final_categories 
                                            ORDER BY	parent_category";
                        cmd.Parameters.Add(new SqlParameter("@uid", SqlDbType.Int)).Value = user_id;

                        using (var dr = cmd.ExecuteReader()) {
                            while (dr.Read()) {
                                var currentCategory = new category();
                                currentCategory.categoryId = Convert.ToInt32(dr["category_id"].ToString());
                                currentCategory.categoryName = dr["category_name"].ToString();
                                currentCategory.parentId= string.IsNullOrEmpty(dr["parent_category"].ToString()) ? -1 : Convert.ToInt32(dr["parent_category"].ToString());
                                myCategories.Add(currentCategory);
                            }
                        }
                        return myCategories;
                    }
                }
            } catch (Exception ex) {
                return myCategories;
            }
        }

        public static List<category> getChildren(category node, List<category> myCategories) {
            var children = new List<category>();
            foreach (var cat in myCategories) {
                if (cat.parentId == node.categoryId) {
                    children.Add(cat);
                }
            }
            return children;
        }

        #endregion

        #region Notes

        public static List<note> getNotesByCategory(int cat_id, int userId) {
            var myNotes = new List<note>();
            int a = -1;
            try {
                using (var con = new SqlConnection(getConString())) {
                    con.Open();
                    using (var cmd = con.CreateCommand()) {

                        //cat_id = 1 --> retrieve all Notes (no category selected) 
                        if (cat_id == 1) {
                            cmd.CommandText = @"SELECT      
	                                                      n.note_id
                                                        , n.note_content
                                                        , n.category_id
                                                        , n.note_title
                                                        , n.note_highlight
                                                        , n.user_id
                                              FROM      [NUDGE].[dbo].[Notes] n
                                              WHERE     n.user_id=@userId
                                              ORDER BY  n.creation_date DESC";
                            cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int)).Value = userId;
                        } else {
                            cmd.CommandText = @"SELECT      
	                                                      n.note_id
                                                        , n.note_content
                                                        , n.category_id
                                                        , n.note_title
                                                        , n.note_highlight
                                                        , n.user_id
                                              FROM      [NUDGE].[dbo].[Notes] n
                                              WHERE     n.category_id=@catId
                                              AND       n.user_id=@userId
                                              ORDER BY  n.creation_date DESC";
                            cmd.Parameters.Add(new SqlParameter("@catId", SqlDbType.Int)).Value = cat_id;
                            cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int)).Value = userId;
                        }

                        using (var dr = cmd.ExecuteReader()) {
                            while (dr.Read()) {
                                var currentNote = new note();
                                currentNote.noteId = Convert.ToInt32(dr["note_id"]);
                                currentNote.categoryId = Convert.ToInt32(dr["category_id"]);
                                currentNote.userId = Convert.ToInt32(dr["user_id"]);
                                currentNote.noteContent = dr["note_content"] as string;
                                currentNote.noteTitle = dr["note_title"] as string;
                                currentNote.noteHighlight = dr["note_highlight"] as string;
                                myNotes.Add(currentNote);
                            }
                        }
                    }
                    con.Close();
                }
                return myNotes;
            } catch (Exception ex) {
                return myNotes;
            }
        }

        public static bool addNote(int userId, int catId, string noteTitle, string noteContent, string noteColor) {
            try {
                using (var con = new SqlConnection(getConString())) {
                    con.Open();
                    using (var cmd = con.CreateCommand()) {
                        cmd.CommandText = @"INSERT INTO     Notes (user_id, category_id, note_title, note_content, note_highlight, creation_date, last_updated_on)
                                            VALUES          (@userid, @catId, @noteTitle, @noteContent, @noteHighlight, @creationDate, @lastUpdatedOn);";

                        cmd.Parameters.Add(new SqlParameter("@userid", SqlDbType.Int)).Value = userId;
                        cmd.Parameters.Add(new SqlParameter("@catId", SqlDbType.Int)).Value = catId;
                        cmd.Parameters.Add(new SqlParameter("@noteTitle", SqlDbType.VarChar, 250)).Value = noteTitle;
                        cmd.Parameters.Add(new SqlParameter("@noteContent", SqlDbType.VarChar, 500)).Value = noteContent;
                        cmd.Parameters.Add(new SqlParameter("@noteHighlight", SqlDbType.VarChar, 50)).Value = noteColor;
                        cmd.Parameters.Add(new SqlParameter("@creationDate", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        cmd.Parameters.Add(new SqlParameter("@lastUpdatedOn", SqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        public static bool deleteNote(int noteId) {
            try {
                using (var con = new SqlConnection(getConString())) {
                    con.Open();
                    using (var cmd = con.CreateCommand()) {
                        cmd.CommandText = @"DELETE FROM notes WHERE note_id=@noteId";

                        cmd.Parameters.Add(new SqlParameter("@noteId", SqlDbType.Int)).Value = noteId;

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        public static bool EditNote(int noteId, string noteTitle, string noteContent, string noteHighlight) {
            try {
                using (var con = new SqlConnection(getConString())) {
                    con.Open();
                    using (var cmd = con.CreateCommand()) {
                        cmd.CommandText = @"UPDATE  Notes 
                                            SET     note_content=@noteC, 
                                                    note_title=@noteT, 
                                                    note_highlight=@noteH, 
                                                    last_updated_on=@lastUpdated
                                            WHERE   note_id=@noteId;";

                        cmd.Parameters.Add(new SqlParameter("@noteId", SqlDbType.Int)).Value = noteId;
                        cmd.Parameters.Add(new SqlParameter("@noteC", SqlDbType.VarChar, 250)).Value = noteContent;
                        cmd.Parameters.Add(new SqlParameter("@noteT", SqlDbType.VarChar, 250)).Value = noteTitle;
                        cmd.Parameters.Add(new SqlParameter("@noteH", SqlDbType.VarChar, 250)).Value = noteHighlight;
                        cmd.Parameters.Add(new SqlParameter("@lastUpdated", SqlDbType.DateTime, 250)).Value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return true;
            } catch (Exception ex) {
                return false;
            }
        }
        #endregion

        #region Tags

        public static List<tag> getTagsByNoteId(int noteId) {
            var myNotes = new List<tag>();
            try {
                using (var con = new SqlConnection(getConString())) {
                    con.Open();
                    using (var cmd = con.CreateCommand()) {
                        cmd.CommandText = @"SELECT 
	                                                      nt.tag_id
	                                                    , nt.tag_name
                                              FROM      [NUDGE].[dbo].[Note_Tags] nt
                                              WHERE     nt.note_id=@nid";

                        cmd.Parameters.Add(new SqlParameter("@nid", SqlDbType.Int)).Value = noteId;

                        using (var dr = cmd.ExecuteReader()) {
                            while (dr.Read()) {
                                var currentTag = new tag();
                                currentTag.tagId = Convert.ToInt32(dr["tag_id"]);
                                currentTag.tagName = dr["tag_name"] as string;
                                myNotes.Add(currentTag);
                            }
                        }
                    }
                    con.Close();
                }
                return myNotes;
            } catch (Exception ex) {
                return myNotes;
            }
        }

        #endregion

        #region Class Declaration

        public class category {
            public int categoryId { get; set; }
            public string categoryName { get; set; }
            public int parentId { get; set; }
        }


        public class note {
            public int noteId { get; set; }
            public string noteContent { get; set; }
            public int categoryId { get; set; }
            public string noteTitle { get; set; }
            public string noteHighlight { get; set; }
            public int userId { get; set; }
        }

        public class tag {
            public int tagId { get; set; }
            public string  tagName { get; set; }
        }

        #endregion

    }
}