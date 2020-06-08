using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.Services;
using System.Web;

using Nudge.Utilities;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace Nudge.Forms {

    public partial class index : Page {

        #region Events
        
        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                var myCategories = AppUtils.getCategories(1);

                hfStringCategories.Value = "";
                for (int i = 0; i < myCategories.Count; i++) {   //foreach root node call children recursively
                    if (myCategories[i].parentId == -1) {
                        RecursiveNodeCall(myCategories[i], myCategories);
                        hfStringCategories.Value += "</ul></li>";
                    }
                }
                divInsertHtml.InnerHtml = hfStringCategories.Value;
                hfStringCategories.Value = "";
            }
        }

        #endregion

        #region Helper Methods

        public void RecursiveNodeCall(AppUtils.category node, List<AppUtils.category> myCategories) {
            var children = AppUtils.getChildren(node, myCategories);
            PrintCategory(node, children.Count == 0 ? false : true);
            for (var i = 0; i < children.Count; i++) {
                RecursiveNodeCall(children[i], myCategories);
                hfStringCategories.Value += "</ul></li>";
            }
        }
        
        public void PrintCategory(AppUtils.category node, bool chevron) {
            hfStringCategories.Value += "<li class='nav-item has-treeview'>" +
                                "<a id=\"" + node.categoryId + "\" href='#' class='nav-link active'> " +
                                    "<i class='nav-icon fas fa-copy'></i>" +
                                    "<p style='color: black; padding-left: 10px'>" +
                                        node.categoryName +
                                        (chevron == false ? "" : ("<span class='glyphicon glyphicon-chevron-left' aria-hidden='true'></span>" +
                                        "<svg class='right bi bi-caret-down-fill' width='1em' height='1em' viewBox='0 0 16 16' fill='currentColor' xmlns='http://www.w3.org/2000/svg'>" +
                                            "<path d='M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 01.753 1.659l-4.796 5.48a1 1 0 01-1.506 0z'/>" +
                                        "</svg>")) +
                                    "</p>" +
                                "</a>" +
                               "<ul class='nav nav-treeview'>";
        }

        [WebMethod(EnableSession = true)]
        public static string DisplayNotes(string stringCatId) {
            var myCategories = AppUtils.getCategories(1);
            int catId = Convert.ToInt32(stringCatId);

            HttpContext.Current.Session["children"] = "";
            GetChildrenByParentId(myCategories.First(i => i.categoryId == catId), myCategories);
            string[] childrenList = HttpContext.Current.Session["children"].ToString().Split(',');
            Array.Resize(ref childrenList, childrenList.Length - 1);

            string printedNotes = "";
            foreach (var child in childrenList) {
                var notesByCat = AppUtils.getNotesByCategory(Convert.ToInt32(child));
                foreach (var note in notesByCat) {
                    var noteTags = AppUtils.getTagsByNoteId(note.noteId);
                    printedNotes +=
                    "<div class='col-lg-4 col-sm-12 col-md-6 col-xs-12'>" +
                        "<div class='card'>" +
                            "<div class='card-header'>" +
                                "<h3 class='card-title'>" + note.noteTitle + "</h3>" +
                                  "<div class=''card-tools'>" +
                                      "<button type='button' class='btn btn-tool' data-card-widget='collapse' data-toggle='tooltip' title = 'Collapse' > " +
                                        "<i class='fas fa-minus'>" + "</i>" +
                                    "</button>" +
                                    "<button type='button' class='btn btn-tool' data-card-widget='remove' data-toggle='tooltip' title = 'Remove' > " +
                                        "<i class='fas fa-times'>" + "</i>" +
                                    "</button>" +
                                "</div>" +
                            "</div>" +
                            "<div class='card-body'>" +
                                note.noteContent +
                            "</div>" +
                            "<div class='card-footer'>";
                    foreach (var tag in noteTags) {
                        printedNotes += "<a href='#'>" + "<span class='border border-secondary rounded p-1'>#" + tag.tagName + "</span></a>";
                    }
                    printedNotes += "</div></div></div>";
                }
            }
            return JsonConvert.SerializeObject(new {
                errorMsg = "SUCCESS",
                notesStringResponse = printedNotes,
                categoryName = myCategories.First(i => i.categoryId == catId).categoryName,
                categoryId = catId
            });
        }

        [WebMethod(EnableSession = true)]
        public static void GetChildrenByParentId(AppUtils.category node, List<AppUtils.category> myCategories) {
            HttpContext.Current.Session["children"] += node.categoryId.ToString() + ",";
            var getParentChildren = AppUtils.getChildren(node, myCategories);
            foreach (var child in getParentChildren) {
                GetChildrenByParentId(child, myCategories);
            }
        }

        [WebMethod]
        public static string AddNote(string catId, string noteTitle, string noteContent, string noteColor) {
            int intCategory = Convert.ToInt32(catId);
            var res = AppUtils.addNote(userId: 1,
                                     catId: intCategory,
                                     noteTitle: noteTitle,
                                     noteContent: noteContent,
                                     noteColor: noteColor);
            if (res == false) {
                return "An error occured.";
            } else {
                return "Successfully added note";
            }
        }
        #endregion

        #region Class Declaration

        #endregion
    }
}