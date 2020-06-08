﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Nudge.Forms.index" %>

<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>Nudge</title>

    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="../src/CSS/bcPicker.css">
    <link rel="stylesheet" href="../Add-ons/CSS/fontawesome/css/all.min.css">
    <link rel="stylesheet" href="https://code.ionicframework.com/ionicons/2.0.1/css/ionicons.min.css">
    <link rel="stylesheet" href="../src/CSS/adminlte.min.css">
    <link href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,400i,700" rel="stylesheet">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" integrity="sha384-9aIt2nRpC12Uk9gS9baDl411NQApFmC26EwAOH8WgZl5MYYxFfc+NcPb1dKGj7Sk" crossorigin="anonymous">
</head>

<body class="hold-transition sidebar-mini sidebar-collapse">
    <form runat="server">
        <div class="wrapper">
            <!-- Navbar -->
            <nav class="main-header navbar navbar-expand navbar-white navbar-light">

                <asp:HiddenField runat="server" ID="hfStringCategories" />
                <asp:HiddenField runat="server" ID="hfCategoryId" />



                <!-- Left navbar links -->
                <ul class="navbar-nav">
                    <li class="nav-item">
                        <a class="nav-link" data-widget="pushmenu" href="#" role="button"><i class="fas fa-bars"></i></a>
                    </li>
                </ul>

                <!-- SEARCH FORM -->
                <ul class="navbar-nav ml-auto">
                    <div class="form-inline ml-3">
                        <div class="input-group input-group-sm">
                            <input class="form-control form-control-navbar" type="search" placeholder="Search" aria-label="Search">
                            <div class="input-group-append">
                                <button class="btn btn-navbar" type="submit">
                                    <i class="fas fa-search"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </ul>
            </nav>

            <!-- Main Sidebar Container -->
            <aside class="main-sidebar sidebar-dark-primary elevation-4">
                <!-- Brand Logo -->
                <a href="#" class="brand-link">
                    <img src="../src/images/AdminLTELogo.png" alt="AdminLTE Logo" class="brand-image img-circle elevation-3"
                        style="opacity: .8">
                    <span class="brand-text font-weight-light">NUDGE</span>
                </a>

                <!-- Sidebar -->
                <div class="sidebar">
                    <div class="user-panel mt-3 pb-3 mb-3 d-flex">
                        <div class="image">
                            <img src="../src/images/user2-160x160.jpg" class="img-circle elevation-2" alt="User Image">
                        </div>
                        <div class="info">
                            <a href="#" class="d-block">Alexander Pierce</a>
                        </div>
                    </div>

                    <!-- Sidebar Menu -->
                    <nav class="mt-2">
                        <ul class="nav nav-pills nav-sidebar flex-column" data-widget="treeview" role="menu" data-accordion="false">
                            <div runat="server" id="divInsertHtml">
                            </div>
                            <li class="nav-header">LABELS</li>
                            <li class="nav-item">
                                <a href="#" class="nav-link">
                                    <i class="nav-icon far fa-circle text-danger"></i>
                                    <p class="text">Important</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="#" class="nav-link">
                                    <i class="nav-icon far fa-circle text-warning"></i>
                                    <p>Warning</p>
                                </a>
                            </li>
                            <li class="nav-item">
                                <a href="#" class="nav-link">
                                    <i class="nav-icon far fa-circle text-info"></i>
                                    <p>Informational</p>
                                </a>
                            </li>
                        </ul>
                    </nav>
                </div>
            </aside>

            <!-- Content Wrapper. Contains page content -->
            <div class="content-wrapper">
                <section class="content-header">
                    <div class="container-fluid">
                        <div class="row mb-2">
                            <div class="col-sm-6">
                                <h1 id="categoryTitle"></h1>
                            </div>
                            <div class="col-sm-6">
                                <input type="image" id="btnAdd" src="../src/images/add.png" style="width: 50px; height: 50px; float: right; margin-right: 50px;" data-toggle="modal" data-target="#modalNote" />
                            </div>
                        </div>
                    </div>
                </section>

                <!-- Main content -->
                <section class="content">
                    <div class="container-fluid">
                        <div class="row" id="divNotes" style="display: none" runat="server">
                        </div>
                    </div>
                </section>
            </div>

            <footer class="main-footer">
                <div class="float-right d-none d-sm-block">
                    <b>Version</b> 1.0
                </div>
                <strong>Copyright &copy; 2020 <a href="http://nudge.al">Nudge.al</a>.</strong> All rights reserved.
            </footer>


        </div>

        <!-- Modal -->
        <div class="modal fade" id="modalNote" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <input type="text" id="txtNoteTitle" class="form-control" placeholder="Title" />
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <textarea class="form-control" id="txtNoteContent" aria-multiline="true" style="height: 200px; width: 100%" placeholder="Content"></textarea>
                    </div>
                    <div class="modal-footer">
                        <div class="color-container">
                            <div id="bcPicker1" class="color-picker border rounded" style="width: 45px; height: 45px; padding: 5px"></div>
                        </div>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-primary" onclick="addNote()">Save</button>
                    </div>
                </div>
            </div>
        </div>

    </form>


    <!-- jQuery -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="../src/JS/bootstrap.bundle.min.js"></script>
    <script src="../src/JS/adminlte.min.js"></script>
    <script src="../src/JS/demo.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js" integrity="sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo" crossorigin="anonymous"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js" integrity="sha384-OgVRvuATP1z7JjHLkuOU7Xw704+h835Lr+6QL9UvYjZE3Ipu6Tp75j7Bh/kR0JKI" crossorigin="anonymous"></script>
    <script type="text/javascript" src="../src/JS/bcPicker.js"></script>
    <script>

        //prevent image button from submiting (default behaviour)
        $('#btnAdd').click(function (event) {
            event.preventDefault();
        });
        $('#bcPicker1').bcPicker({
            colors: [
                '35373B', '585218', '3A2353', '2E4F1D', '164642', '522625'
            ]
        });

        $(".nav-link.active").click(function () {
            var catid = $(this).attr("id");
            getNotesByCatId(catid);
        });
        function getNotesByCatId(catId) {
            bindNodes(catId, function (errorMsg, notesStringResponse, categoryName, hfCategoryId) {
                if (errorMsg != 'SUCCESS') {
                    showMessage(errorMsg);
                    return;
                }
                $("#<%=divNotes.ClientID%>").hide();
                $("#<%=divNotes.ClientID%>").html("");
                $("#<%=divNotes.ClientID%>").delay(300).slideDown();
                document.getElementById('<%=divNotes.ClientID%>').insertAdjacentHTML('beforeend', notesStringResponse);
                $('#categoryTitle').html(categoryName);
                $('#<%=hfCategoryId.ClientID%>').val(hfCategoryId);
            });
        }
        function bindNodes(catid, callback) {
            var request = JSON.stringify({
                stringCatId: catid,
            });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: "index.aspx/DisplayNotes",
                data: request,
                success: function (res) {
                    var data = JSON.parse(res.d);
                    callback(data.errorMsg, data.notesStringResponse, data.categoryName, data.categoryId);
                },
                error: function (jqXHR, status, errorThrown) {
                    showMessage("An error occured. Sorry!");
                }
            });
        }

        function addNote() {
            var category = 1; //initialize category id = 1 and check if another category is selected--> (1=Uncategorized)
            if ($('#<%=hfCategoryId.ClientID%>').val() != "") {
                category = $('#<%=hfCategoryId.ClientID%>').val();
            }
            var bgcolor = $.fn.bcPicker.toHex($('.bcPicker-picker').css("background-color"));
            var request = JSON.stringify({
                catId: category,
                noteTitle: $('#txtNoteTitle').val(),
                noteContent: $('#txtNoteContent').val(),
                noteColor: bgcolor
            });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                url: "index.aspx/AddNote",
                data: request,
                success: function (res) {
                    showMessage(res.d);
                    getNotesByCatId(category);
                },
                error: function (jqXHR, status, errorThrown) {
                    showMessage("An error occured. Sorry!");
                }
            });
        }

        function showMessage(msg) {
            alert(msg);
        }

    </script>
</body>

</html>
k