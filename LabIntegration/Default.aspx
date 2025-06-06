<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MoviePicker.Default" Async="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Персональные подборки фильмов</title>
    <!-- Bootstrap CDN -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #f9f9f9;
        }

        .form-section {
            max-width: 600px;
            margin: 30px auto;
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 0 15px rgba(0,0,0,0.1);
        }

        .poster-img {
            width: 80px;
            border-radius: 6px;
        }

        .table > tbody > tr > td {
            vertical-align: middle;
        }

        .btn-primary {
            width: 100%;
        }

        .grid-section {
            margin: 40px auto;
            max-width: 900px;
        }

        h2 {
            text-align: center;
            margin-bottom: 25px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="form-section">
                <h2>Создать подборку фильмов</h2>

                <div class="mb-3">
                    <asp:Label ID="lblGenre" runat="server" Text="Жанр:" CssClass="form-label" />
                    <asp:DropDownList ID="ddlGenre" runat="server" CssClass="form-select" />
                </div>

                <div class="mb-3">
                    <asp:Label ID="lblMinRating" runat="server" Text="Минимальный рейтинг (от 0 до 10):" CssClass="form-label" />
                    <asp:TextBox ID="txtMinRating" runat="server" CssClass="form-control" Text="7" />
                </div>

                <div class="mb-3">
                    <asp:Label ID="lblYearFrom" runat="server" Text="Год выпуска от:" CssClass="form-label" />
                    <asp:TextBox ID="txtYearFrom" runat="server" CssClass="form-control" Text="2000" />
                </div>

                <div class="mb-3">
                    <asp:Label ID="lblYearTo" runat="server" Text="Год выпуска до:" CssClass="form-label" />
                    <asp:TextBox ID="txtYearTo" runat="server" CssClass="form-control" Text="2023" />
                </div>

                <asp:Button ID="btnSearch" runat="server" Text="Найти фильмы" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>

            <div class="grid-section">
                <asp:GridView ID="gvMovies" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered mt-4" Visible="false">
                    <Columns>
                        <asp:BoundField DataField="Title" HeaderText="Название" />
                        <asp:BoundField DataField="Year" HeaderText="Год" />
                        <asp:BoundField DataField="Rating" HeaderText="Рейтинг" />
                        <asp:TemplateField HeaderText="Постер">
                            <ItemTemplate>
                                <asp:Image ID="imgPoster" runat="server" CssClass="poster-img" ImageUrl='<%# Eval("Poster") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:HyperLinkField DataNavigateUrlFields="Trailer" DataNavigateUrlFormatString="{0}" Text="Трейлер" HeaderText="Смотреть" />
                    </Columns>
                </asp:GridView>

                <asp:Button ID="btnSaveJson" runat="server" Text="Сохранить в JSON" CssClass="btn btn-success mt-3" OnClick="btnSaveJson_Click" Visible="false" />
            </div>
        </div>
    </form>

    <!-- Bootstrap JS (необязательно, если не используете интерактивные компоненты) -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
