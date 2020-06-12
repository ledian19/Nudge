using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.Services;
using System.Web;

using Nudge.Utilities;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Text;

namespace Nudge.Forms {

    public partial class index : Page {

        #region Events
        
        protected void Page_Load(object sender, EventArgs e) {
            if (!IsPostBack) {
                var myCategories = AppUtils.getCategories(1);
                var root = new Node();
                root.catName = "root";
                root.id = 0;
                root.children = new List<Node>();
                categories.Add(root);
                hfStringCategories.Value = "";
                for (int i = 0; i < myCategories.Count; i++) {   //foreach root node call children recursively
                    if (myCategories[i].parentId == -1) {
                        callJsTree(myCategories[i], myCategories, root);
                        RecursiveNodeCall(myCategories[i], myCategories);
                        hfStringCategories.Value += "</ul></li>";
                    }
                }
                divInsertHtml.InnerHtml = hfStringCategories.Value;
                hfStringCategories.Value = "";
                DisplayNotes("1");
            }
        }

        #endregion

        #region Category management methods

        public static List<Node> categories = new List<Node>();

        //public List<Node> callJsTree(List<AppUtils.category> myCategories) {
        //    var finalCategories = new List<Node>();
        //    var list = myCategories.Where(f => f.parentId == -1);
        //    foreach (var parent in list) {
        //        var newNode = new Node();
        //        newNode.id = parent.categoryId;
        //        newNode.catName = parent.categoryName;
        //        finalCategories.Add(newNode);
        //    }
        //    foreach (var child in myCategories.Where(f => f.parentId != -1)) {
        //        var parent = finalCategories.First(i => i.id == child.parentId); //cannot go deeper than level 2, unable to retrieve parent (not in finalCategories list  
        //        if (parent.children == null) {
        //            parent.children = new List<Node>();
        //        }
        //        var childToAdd = new Node();
        //        childToAdd.id = child.categoryId;
        //        childToAdd.catName = child.categoryName;
        //        parent.children.Add(childToAdd);
        //    }
        //    return finalCategories;
        //}

        public void callJsTree(AppUtils.category node, List<AppUtils.category> myCategories, Node parent) {
            var children = AppUtils.getChildren(node, myCategories);
            var newParent = addNode(node, parent);
            //----------
            for (var i = 0; i < children.Count; i++) {
                callJsTree(children[i], myCategories, newParent); //call children of current node
            }
        }
        
        public Node addNode(AppUtils.category child, Node parent) {
            var newChild = new Node();
            newChild.catName = child.categoryName;
            newChild.id = child.categoryId;
            if (parent.children == null) {
                parent.children = new List<Node>();
            }
            parent.children.Add(newChild);
            return newChild;
        }

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
                                "<a id=\"" + node.categoryId + "\" href='#' class='nav-link'> " +
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

        #endregion

        #region Note management WebMethods

        [WebMethod(EnableSession = true)]
        public static string DisplayNotes(string stringCatId) {
            var myCategories = AppUtils.getCategories(1);
            int catId = Convert.ToInt32(stringCatId);

            HttpContext.Current.Session["children"] = "";
            GetChildrenByParentId(myCategories.First(i => i.categoryId == catId), myCategories);
            string[] childrenList = HttpContext.Current.Session["children"].ToString().Split(',');
            Array.Resize(ref childrenList, childrenList.Length - 1);

            var printedNotes = new StringBuilder();

            foreach (var child in childrenList) {
                var notesByCat = AppUtils.getNotesByCategory(Convert.ToInt32(child), 1);
                foreach (var note in notesByCat) {
                    var noteTags = AppUtils.getTagsByNoteId(note.noteId);
                    printedNotes.Append("<div class='col-lg-4 col-sm-12 col-md-6 col-xs-12'>" +
                                "<div class='card' style='background-color: " + note.noteHighlight + ";'>" +
                                    "<div class='card-header' style='background-color: " + note.noteHighlight + "; border: none'>" +
                                        "<h3 class='card-title text-bold'>" + note.noteTitle + "</h3>" +
                                        "<div class=''card-tools'>" +
                                            "<button type='button' class='btn btn-tool' data-card-widget='collapse' data-toggle='tooltip' title = 'Collapse' style='float:right'> " +
                                                "<i class='fas fa-minus'>" + "</i>" +
                                            "</button>" +
                                        "</div>" +
                                    "</div>" +
                                    "<div class='card-body'>" +
                                        "<p>" +
                                        note.noteContent +
                                        "</p>");
                    foreach (var tag in noteTags) {
                        printedNotes.Append("<a href='#'>" + "<span class='border border-secondary rounded p-1'>" + tag.tagName + "</span></a>");
                    }
                    printedNotes.Append("</div>" +
                                    "<div class='card-footer' style='border: none; background-color: " + note.noteHighlight + ";'>" +
                                        "<button type='button' class='btn btn-tool p-1' style='float: right'>" +
                                        "    <i class='fas fa-ellipsis-v'></i>" +
                                        "</button>" +
                                        "<button type='button' onclick='initializeEditModal(" + note.noteId + ", \"" + note.noteTitle + "\", \"" + note.noteContent + "\", \"" + note.noteHighlight + "\", " + note.categoryId + ")' class='btn btn-tool p-1' style='float: right'>" +
                                        "    <i class='fas fa-edit'></i>" +
                                        "</button>" +
                                        "<button type='button' onclick='deleteNote(" + note.noteId + ")' class='btn btn-tool p-1' style='float: right'>" +
                                        "    <i class='fas fa-trash-alt'></i>" +
                                        "</button>" +
                                        "<button type='button' class='btn btn-tool p-1' style='float: right'>" +
                                        "    <i class='fas fa-bookmark'></i>" +
                                        "</button>" +
                                    "</div>" +
                                "</div>" +
                            "</div>");
                }
            }
            return JsonConvert.SerializeObject(new {
                errorMsg = "SUCCESS",
                notesStringResponse = printedNotes.ToString(),
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

        [WebMethod]
        public static string DeleteNote(int noteId) {
            var res = AppUtils.deleteNote(noteId);
            if (res == false) {
                return "An error occured.";
            } else {
                return "Successfully deleted note";
            }
        }
        [WebMethod]
        public static string EditNote(int noteId, string noteTitle, string noteContent, string noteHighlight) {
            var res = AppUtils.EditNote(noteId, noteTitle, noteContent, noteHighlight);
            if (res == false) {
                return "An error occured.";
            } else {
                return "Success";
            }
        }
        #endregion


        public class Node {
            public string catName { get; set; }
            public decimal id { get; set; }
            public NodeState state { get; set; }
            public List<Node> children { get; set; }
        }

        public class NodeState {
            public bool opened { get; set; }
            public bool disabled { get; set; }
            public bool selected { get; set; }

            public NodeState(bool opened = true,
                             bool disabled = false,
                             bool selected = false) {
                this.opened = opened;
                this.disabled = disabled;
                this.selected = selected;
            }
        }
    }
}