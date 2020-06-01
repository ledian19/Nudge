using System;
using Nudge.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace Nudge.Forms {

    public partial class index : Page {

        #region Events

        /* TODO in the later version
         1. Get child by parent_id.
         2. foreach(child)--> hasChildren ? Step 1 : insertChild in childArray
         3. look for notes with catID in childArray
        */


        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                var myCategories = AppUtils.getCategories(1);
                var res = AppUtils.getChildren(myCategories[0], myCategories);
                count.Value = "";

                for (int i = 0; i < myCategories.Count; i++) {   //foreach root node call children recursively
                    if (myCategories[i].parentId == -1) {
                        recursiveNodeCall(myCategories[i], myCategories);
                        count.Value += "</ul></li>";
                    }
                }
                divInsertHtml.InnerHtml = count.Value;
                count.Value = "";
            }
        }

        #endregion

        #region Helper Methods

        public void recursiveNodeCall(AppUtils.category node, List<AppUtils.category> myCategories) {
            var children = AppUtils.getChildren(node, myCategories);
            printNode(node, children.Count == 0 ? false : true);
            for (var i = 0; i < children.Count; i++) {
                recursiveNodeCall(children[i], myCategories);
                count.Value += "</ul></li>";
            }
        }

        public void printNode(AppUtils.category node, bool chevron) {
            count.Value += "<li class='nav-item has-treeview'>" +
                                "<a id=\"" + node.categoryId + "\" href='#' class='nav-link active'> " +
                                    "<i class='nav-icon fas fa-copy'></i>"+
                                    "<p style='color: black; padding-left: 10px'>" +
                                        node.categoryName +
                                        (chevron==false ? ""  : ("<span class='glyphicon glyphicon-chevron-left' aria-hidden='true'></span>" +
                                        "<svg class='right bi bi-caret-down-fill' width='1em' height='1em' viewBox='0 0 16 16' fill='currentColor' xmlns='http://www.w3.org/2000/svg'>" +
                                            "<path d='M7.247 11.14L2.451 5.658C1.885 5.013 2.345 4 3.204 4h9.592a1 1 0 01.753 1.659l-4.796 5.48a1 1 0 01-1.506 0z'/>"+
                                        "</svg>")) +
                                    "</p>" +
                                "</a>" +
                               "<ul class='nav nav-treeview'>";
        }


        [WebMethod]
        public static string displayNotes(int catId) {
            string printedNotes = "";
            var myNotes = AppUtils.getNotesByCategory(catId);
            foreach (var note in myNotes) {
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
            return printedNotes;
        }
        
        #endregion


        #region Class Declaration


        #endregion
    }
}