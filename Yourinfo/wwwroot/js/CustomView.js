$(document).ready(function () {
    $(".mobile1,.cli,.wli,.sli,.eli,.mapli,.webli,.pli,.bli").hide();
    $(".myForm11 :input").prop("disabled", true);
});

$("#btnhhs").change(function () {
    var isbusiness = $('#btnhhs option:selected').val();
    if (isbusiness == '1') {
        $(".myForm11 :input").prop("disabled", false);
        $("#main-category").val("Business");
        $(".gst_desig_label").text("GSTIN Number");
        $(".owner-full-name").text("Owner / Founder Name");
        $(".cname, .ownername-business ,.ownername1").show();
        var gst = $('#gstnumber').val();
        $(".fullname-individual,.companyname-individual").hide();
        $(".gst2").text("GSTIN :" + gst);
    }
    else {
        $(".myForm11 :input").prop("disabled", false);
        $("#main-category").val("Individual");
        $(".owner-full-name").text("Full Name");
        $(".gst_desig_label").text("Designation");
        $(".cname, .ownername-business ,.ownername1").hide();
        // $(".cname").hide();
        var gst = $('#gstnumber').val();

        $(".fullname-individual,.companyname-individual").show();
        $(".gst2").text(gst);
    }
});