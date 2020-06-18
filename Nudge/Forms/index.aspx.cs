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
                var categoriesList = new List<Node>();
                var root = new Node();
                root.text = "Notes";
                root.id = 1;
                root.icon = "fas fa-folder-open pt-0";
                root.children = new List<Node>();
                categoriesList.Add(root);
                for (int i = 0; i < myCategories.Count; i++) {   //foreach root node call children recursively
                    if (myCategories[i].parentId == -1 && myCategories[i].categoryId != 1) {
                        callJsTree(myCategories[i], myCategories, root);
                    }
                }
                hfTreeData.Value = JsonConvert.SerializeObject(categoriesList);
                DisplayNotes("1");
            }
        }

        #endregion

        #region Category management methods

        public void callJsTree(AppUtils.category node, List<AppUtils.category> myCategories, Node parent) {
            var parentToBe = addNode(node, parent);
            var children = AppUtils.getChildren(node, myCategories);
            for (var i = 0; i < children.Count; i++) {
                callJsTree(children[i], myCategories, parentToBe); //call children of current node
            }
        }
        
        public Node addNode(AppUtils.category child, Node parent) {
            var newChild = new Node();
            newChild.text = child.categoryName;
            newChild.id = child.categoryId;
            newChild.icon = "fas fa-folder-open pt-0";
            if (parent.children == null) {
                parent.children = new List<Node>();
            }
            parent.children.Add(newChild);
            return newChild;
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
            public string text { get; set; }
            public decimal id { get; set; }
            public string icon { get; set; }
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