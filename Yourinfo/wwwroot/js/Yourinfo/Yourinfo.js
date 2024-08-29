 /* Default styles for the div */

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

function funnumberverify() {

    if ($('#numbercheck').is(':checked')) {
        var mobile = $('#mobile').val();
        $('#whatsappnumber').val(mobile)
            ;
    }
    else $('#whatsappnumber').val('');
}

$(function () {
    $("#galimg").on("change", function () {
        $('#submit').attr('disabled', 'disabled');


        var form_data = new FormData();

        // Read selected files
        var totalfiles = document.getElementById('galimg').files.length;
        if (totalfiles > 20) { alert('Sorry You can upload only 20 files'); return false; }
        if (totalfiles == 0) return;
        for (var index = 0; index < totalfiles; index++) {
            form_data.append("files[]", document.getElementById('galimg').files[index]);
        }

        // AJAX request
        $.ajax({
            url: 'uploadmultiple.php',
            type: 'post',
            beforeSend: function () { $('#pwait').html('Please Wait While Images Uploading...'); },
            data: form_data,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != 'Error in uploading..') {
                    $('#images').val(response);
                    $('#submit').removeAttr('disabled');
                    $('#pwait').css('color', 'green');
                    $('#pwait').html('Uploaded');
                } else { $('#pwait').html(response); }
            }
        });

    });
})

function fungetscat(datavalue) {
    $.ajax({
        url: 'getscat.php',
        type: 'POST',
        data: { id: datavalue },
        success: function (result) {
            $('#subcategory').html(result);
        }
    });

}
$(document).ready(function () {

    var countryOptions = '';
    var stateOptions = '';
    var cityOptions = '';
    var i = 0;
    $("#country").find("option").remove();
    $.getJSON('countries.json', function (data) {
        $("#country").append('<option value="0">Select Country Name</option>');
        $(data.countries).each(function (x, y) {
            $("#country").append('<option value="' + y.id + '">' + y.name + '</option>')
        })



    });
});

$(function () {
    $("#country").change(function () {
        var id = $(this).val();
        if (id != "0") {

            $.getJSON('countries.json', function (data) {
                //$("#states").append('<option value="0">Select </option>');
                $(data.countries).each(function (x, y) {
                    if (y.id == id) {
                        $('#countrycode').val('+' + y.phoneCode);
                    }
                });




            });
        }
    })
});


$(function () {
    $("#country").change(function () {
        var id = $(this).val();
        if (id != "0") {
            $("#states").find("option").remove();
            $.getJSON('states.json', function (data) {
                $("#states").append('<option value="0">Select State</option>');
                console.log(data.states.filter(function (i, n) {
                    return n.country_id == id;
                }))
                $(data.states).each(function (x, y) {
                    if (y.country_id == id)
                        $("#states").append('<option value="' + y.id + '">' + y.name + '</option>')
                })



            });
        }
    })
});

$(function () {
    $("#states").change(function () {
        var id = $(this).val();
        if (id != "0") {
            $("#city").find("option").remove();
            $.getJSON('cities.json', function (data) {
                $("#city").append('<option value="">Select City</option>');

                $(data.cities).each(function (x, y) {
                    if (y.state_id == id)
                        $("#city").append('<option value="' + y.id + '">' + y.name + '</option>')
                })



            });
        }
    })
});
window.bool = false;
function mysubmit() {
    var isbusiness = $('.btnhhs').attr('data-isbusiness');
    var pass = $('#pass').val();
    var cpass = $('#cpass').val();
    var email = $('#emailx').val();
    if (pass != cpass) {
        alert('password do not match');
        return false;
    }
    else if (!window.bool) {
        $.ajax({
            url: "checkemail.php",
            type: "POST",
            data: { email: email },
            success: function (html) {
                if (html == 'sorry') { window.bool = false; alert('Sorry Email Already Exists'); }
                else { window.bool = true; document.getElementById("myForm11").submit(); }

            }
        });

    }
    return window.bool;





}
function mylogin() {
    var email = $('#loginemail').val();
    var pass = $('#loginpass').val();
    $.ajax({
        url: "loginuser.php",
        type: "POST",
        data: { email: email, pass: pass },
        success: function (html) {
            if (html == 'Success') {
                window.location.href = 'edit.php';
            }
            else { alert('User name or password incorrect'); }

        }
    });

}
$(document).ready(function () {
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });
});

window.basecolor = "#caa11c";
window.states = ["Select State", "Andaman & Nicobar Islands", "Andhra Pradesh", "Arunachal Pradesh",
    "Assam", "Bihar", "Chandigarh", "Chhattisgarh", "Dadra & Nagar Haveli", "Daman & Diu", "Delhi", "Goa", "Gujarat", "Haryana", "Himachal Pradesh", "Jammu & Kashmir", "Jharkhand", "Karnataka", "Kerala", "Lakshadweep", "Madhya Pradesh", "Maharastra", "Manipur", "Meghalaya", "Mizoram", "Nagaland", "Orissa", "Pondicherry", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu", "Tripura", "Uttar Pradesh", "Uttarakhand", "West Bengal"];
window.isbusiness = 1;
window.ow = function (res) {
    $('#sxwoner').html(res + " : ");
    $("#owname").html(res);
}
window.aboutss = function (res) {
    $("#abtservice,#iopltype").html(res);
}
window.deft = function (reft) {
    window.basecolor = $(reft).attr("data-color");
    $('.xheader,.rightx').css("background", window.basecolor);
    $('#sxbusinessname').css("color", window.basecolor);
}
window.ki = function () {
    var states = $("#states").val();
    var city = $("#city").val();
    states = (states == "Not Applicable" ? "" : ", " + states);
    city = (city == "Not Applicable" ? "" : ", " + city);

    var f = $('#addressline1').val() + city + states + ", " + $('#pincode').val() + ($("#countrycode").val() == "91" ? ", India" : "") + "<br>";
    if ($("#mobile").val() != "")
        f += $("#countrycode").val() + "-" + $('#mobile').val();
    if ($("#email").val() != "")
        f += " | " + $('#email').val();
    $('#sxaddress').html(f);
}

window.getcityname = function () {
    ki();
}
$(function () {
    $.post("c.php", {}, function (res) {
        $(res).each(function (x, y) {
            $("#countrycode").append("<option " + (y.code == "91" ? "selected" : "") + " value='" + y.code + "'>" + y.name + "(" + y.code + ")</option>");
        })
    }, "JSON");
    $("#countrycode").change(function () {

        if ($(this).val() == "91") {
            $("#states,#city").find("option").remove();
            $("#states").append("<option value='Maharastra'>Maharastra</option>");
            $("#city").append("<option value='Mumbai'>Mumbai</option>");
        }
        else {
            $("#states,#city").find("option").remove();
            $("#states").append("<option value='Not Applicable'>Not Applicable</option>");
            $("#city").append("<option value='Not Applicable'>Not Applicable</option>");

        }
        ki();
    })
    $('#save').click(function () {
        savedata();
    })
    $('#lockups').click(function () {
        $('#myModal').modal("show");
    })
    $('#facebook').keyup(function () {
        if ($.trim($(this).val()).length != 0)
            $('#linkfacebook').show();
        else $('#linkfacebook').hide();

    });
    $('#linkedin').keyup(function () {
        if ($.trim($(this).val()).length != 0)
            $('#linklinkedin').show();
        else $('#linklinkedin').hide();

    });
    $('#youtube').keyup(function () {
        if ($.trim($(this).val()).length != 0)
            $('#linkyoutube').show();
        else $('#linkyoutube').hide();
    });
    $('#twitter').keyup(function () {
        if ($.trim($(this).val()).length != 0)
            $('#linktwitter').show();
        else $('#linktwitter').hide();
    });
    $('#instagram').keyup(function () {

        if ($.trim($(this).val()).length != 0)
            $('.linkinstagram').show();
        else $('.linkinstagram').hide();
    });
    $('#designation').change(function () {
        $('#sxownername').html($(this).val());
    })
    $("#designationtext").keyup(function () {
        $('#sxownername').html($(this).val());
        if ($.trim($(this).val()).length != 0)
            $('#sxwoner,#sxownername').show();
        else $('#sxwoner,#sxownername').hide();
    })
    $("#mobile,#emailx,#pincode,#addressline1").keyup(function () {
        ki();
        if ($.trim($("#mobile").val()).length != 0)
            $('#dcallnow,#dsms').show();
        else $('#dcallnow,#dsms').hide();

        if ($.trim($("#emailx").val()).length != 0)
            $('#demail').show();
        else $('#demail').hide();
    });


    $("#mobile").keyup(function () {

        if ($.trim($(this).val()).length == 0) {
            $(".cli").hide();
        }
        else {
            $(".cli").show();

        }
    });

    $('#fullname').keyup(function () {
        $('.cname').html($(this).val());
    });
    
    $('.xheader,.rightx').css("background", window.basecolor);
    $('.center').css("color", window.basecolor);
    var colors = ["#caa11c", "#2a0000", "#327848", "#080027", "#651026", "#ff0042", "#363636", "#000000", "#873600", "#ff00d1", "#660063", "#8100c6", "#00195b", "#0046ff", "#a20000", "#009c80", "#2ea000", "#ffb900", "#ff7800", "#f20000"];
    var html = "<table><tr>";
    $(colors).each(function (x, y) {
        html += " <td   style='background:" + y + ";cursor:pointer'><label style='padding:15px'><input onchange='deft(this)' " + (x == 0 ? "checked" : "") + " data-color='" + y + "' type='radio' value=" + y + " name='colors' class='colors'/></label></td>";
    });
    html += "</tr></table>";
    $('.sdde').html(html);

    $("#btnhhs").change(function () {

        //$(".btnhhs").removeClass("btn-primary").addClass("btn-default");
        //$(this).addClass("btn-primary");
        var isbusiness = $('#btnhhs option:selected').val();

        document.getElementById('isbussunessval').value = isbusiness;
        if (isbusiness == "0") {
            $('#businessname').prop('required', false);
            $('#ownername').prop('required', true);
            $('#sxwoner').html("Designations : ")
            $('#logoor').hide();
            $('#comname').show();
            $('#bname').hide(); $('.gst').hide();
            $('#cname').hide();
            $('#dname').show();
            $('#btnhhs1').css('background-color', 'red');
            $('#btnhhs2').css('background-color', 'green');
            //$('#sxownername').html("Owner")

            // $(".businessname,.owner,.ownername,.gstnumber").hide();
            $('.accordion').css('margin-top', '25px');
            // $(".fullname,.designation,.companyname").show();
            //                 $('#sxgstin').html("Company Name");
        }
        else {
            $(".fullname,.designation,.companyname").hide();
            $('#businessname').prop('required', true);
            $('#ownername').prop('required', false);
            $('#logoor').show();
            $('#comname').hide();
            $('#bname').show();
            $('#cname').show();
            $('#dname').hide();
            $('#btnhhs2').css('background-color', 'red');
            $('#btnhhs1').css('background-color', 'green');

            // $(".businessname,.owner,.ownername,.gstnumber").show();       $('#sxwoner').html("Owner");
            // $('#sxgstin').html("GSTIN");

        }
    });
    window.iskk = false;
    $('.asw').click(function () {
        if (!window.iskk) {
            window.iskk = true;
            $('.rightx').show();
        }
        else {
            window.iskk = false;
            $('.rightx').hide();
        }
    })
});

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#edrimg')
                .attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);
        initFileOnlyAjaxUpload();
    }
}
window.addresssx = function (rf) {
    $('#sxaddressde').html(rf);
}

window.savedata = function () {
    var obj = {
        businessname: $.trim($('#businessname').val()),
        owner: $('#ow').val(),
        ownername: $.trim($('#ownername').val()),
        gstnumber: $.trim($('#gstnumber').val()),
        aboutss: $.trim($('#aboutss').val()),
        aboutus: $.trim($('#aboutus').val()),
        mobile: $.trim($('#mobile').val()),
        whatsappnumber: $.trim($('#whatsappnumber').val()),
        email: $.trim($('#emailx').val()),
        password: $.trim($('#passwordx').val()),
        confirmpassword: $.trim($('#confirmpasswordx').val()),
        addresssx: $.trim($('#addresssx').val()),
        addressline1: $.trim($('#addressline1').val()),
        states: $.trim($('#states').val()),
        city: $.trim($('#city').val()),
        pincode: $.trim($('#pincode').val()),
        maplink: $.trim($('#maplink').val()),
        website: $.trim($('#website').val()),
        paynow: $.trim($('#paynow').val()),
        facebook: $.trim($('#facebook').val()),
        linkedin: $.trim($('#linkedin').val()),
        twitter: $.trim($('#twitter').val()),
        youtube: $.trim($('#youtube').val()),
        instagram: $.trim($('#instagram').val()),
        designation: $("#designationtext").val(),
        fullname: $('#fullname').val(),
        companyname: $.trim($('#companyname').val()),
        customfield: $.trim($("#customfield").val()),
        image: window.image,
        color: window.basecolor, type: window.isbusiness
    };
    $.post("save.php", obj, function (res) {
        $("#hashs").attr("href", "pdf.php?hash=" + res.hash);

        $('#myInputzz,#serd').html("http://mobocab.dibsinfotech.com/a.php?mobile=" + res.mobile);

        $("#myModal1").modal("show");

    }, "JSON");
}


function cll() {
    copyToClipboard();
}


var copyToClipboard = function () {
    var $body = document.getElementsByTagName('body')[0];
    var secretInfo = document.getElementById('myInputzz').innerHTML;
    var $tempInput = document.createElement('INPUT');
    $body.appendChild($tempInput);
    $tempInput.setAttribute('value', secretInfo)
    $tempInput.select();
    document.execCommand('copy');
    $body.removeChild($tempInput);
    alert("Successfully Copied")
}

$(document).ready(function () {
    $("#businessname").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var cname = $('#businessname').val();
        $('.cname').text(cname);
        $('.companyname-indi').text(cname);
    });

    $("#gstnumber").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var gst = $('#gstnumber').val();
        $(".gst2").text(gst);

    });

    $("#ownername").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var owner = $('#ownername').val();
        $(".ownername1").text(owner);
        $(".fullname-individual").text(owner);

    });
    //  $("#fullname").keyup(function(){
    //     $('.ss').hide();
    //    $('.content').show();
    //    var fullname = $('#fullname').val();
    // $("#name").text(fullname);
    // $("#name1").text(fullname);

    //  });

    $("#designationtext").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var bname = $('#designationtext').val();
        $(".bname").text(bname);
        // $("#bname1").text(bname);

    });

    $("#companyname").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var comname = $('#companyname').val();
        $("#comname").text(comname);
        $(".company-name").text(comname);

    });

    // ==========================



    // ==========================
    // ==========================CUSTOM COLORS JS==============================
    //   $(".colors").change(function(){
    // var tprice = $('.colors').val();
    // $('.bh4').css('color', tprice);

    // });

    //   $("input[type='button']").click(function(){
    //   var radioValue = $("input[name='colors']:checked").val();
    //     if(radioValue){
    //         alert("Your are a - " + radioValue);
    //     }
    // });
    // var xxx=$('#facebook').val();

    $("#mobile").keyup(function () {
        var vall = $('#mobile').val();
        if (vall != "") {
            // $(".mobile1,.cli,.wli,.sli").attr("href", vall);
            $(".mobile1,.cli,.sli").show();
            $(".mobile1").text(vall);
            $(".mobiletext").text(vall);
            $(".whatsappnumber1").text((vall));
        }
        else {
            // $(".mobile1,.cli,.wli,.sli").css("display", "none");
            $(".mobile1,.cli,.sli").hide();
        }
    });

    $("#whatsappnumber").keyup(function () {
        var vall = $('#whatsappnumber').val();
        $('.ss').hide();
        $('.content').show();
        if (vall != "") {
            $(".wli").show();
            $(".whatsappnumber1").text(vall);
        }
        else { $(".wli").hide(); }
    });



    $("#maplink").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#maplink').val();
        if (vall != "") {
            // $(".mobile1,.cli,.wli,.sli").attr("href", vall);
            $(".mapli").show();
        }
        else {
            $(".mapli").hide();
        }
    });

    $("#website").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#website').val();
        if (vall != "") {
            $(".webli").show();
            $(".website1").text(vall);
        }
        else {
            $(".webli").hide();
        }
    });

    $("#paynow").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#paynow').val();
        if (vall != "") { $(".pli").show(); }
        else { $(".pli").hide(); }
    });

    $("#brochure").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#brochure').val();
        if (vall != "") { $(".bli").show(); }
        else { $(".bli").hide(); }
    });

    $("#facebook").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#facebook').val();
        if (vall != "") {
            $(".facebook1").css("display", "inline-block");
            $(".facebook1").attr("href", vall);
        }
        else {
            $(".facebook1").css("display", "none");
        }
    });

    $("#twitter").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#twitter').val();
        if (vall != "") {
            $(".twitter1").css("display", "inline-block");
            $(".twitter1").attr("href", vall);
        }
        else {
            $(".twitter1").css("display", "none");
        }
    });

    $("#youtube").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#youtube').val();
        if (vall != "") {
            $(".youtube1").css("display", "inline-block");
            $(".youtube1").attr("href", vall);
        }
        else {
            $(".youtube1").css("display", "none");
        }
    });

    $("#linkedin").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#linkedin').val();
        if (vall != "") {
            $(".linkedin1").css("display", "inline-block");
            $(".linkedin1").attr("href", vall);
        }
        else {
            $(".linkedin1").css("display", "none");
        }
    });
    $("#instagram").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var vall = $('#instagram').val();
        if (vall != "") {
            $(".instagram1").css("display", "inline-block");
            $(".instagram1").attr("href", vall);
        }
        else {
            $(".instagram1").css("display", "none");
        }
    });
    $("#emailx").keyup(function () {
        var vall = $('#emailx').val();
        $('.ss').hide();
        $('.content').show();
        if (vall != "") {
            $(".emailx1").css("display", "inline-block");
            $(".emailx1").attr("href", vall);
            $(".emailtext").text(vall);
            $(".eli").show();

        }
        else {
            $(".emailx1").css("display", "none");
            $(".eli").show();
        }
    });
    $("#passwordx").keyup(function () {
        $('.ss').hide();
        $('.content').show();
    });
    $("#confirmpasswordx").keyup(function () {
        $('.ss').hide();
        $('.content').show();
    });

    $("#customfield").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var customfieldval = $('#customfield').val();
        $(".customfield1").text(customfieldval);
    });

    var header_style = null;
    $("input[name='header-style']").change(function () {
        $('.ss').hide();
        $('.content').show();
        header_style = this.value;

        if (header_style == "1") {
            $(".abc").css("display", "none");
            $(".top-border1").css("display", "block");
        }
        if (header_style == "2") {
            $(".top-border1").css("display", "none");
            $(".abc").css("display", "block");
        }
        if (header_style == "0") {
            $(".top-border1").css("display", "none");
            $(".abc").css("display", "none");
        }

    });

    $("#aboutss").change(function () {
        $('.ss').hide();
        $('.content').show();
        var aboutssval = $(this).children("option:selected").val();

        $(".aboutss1").text(aboutssval);
    });

    $("#aboutus").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var aboutusval = $('#aboutus').val();
        $(".aboutus1").text(aboutusval);
    });

    $("#addressline1").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var addressline1val = $('#addressline1').val();
        if (addressline1val != "") { $(".addressline11").text(addressline1val); }
        else { $(".addressline11").text("Enter Address here"); }
    });

    $("#countrycode").change(function () {
        $("#passwordx").keyup(function () {
            $('.ss').hide();
            $('.content').show();
        });
        var countrycodeval = $(this).children("option:selected").val();
        $(".countrycode1").text(countrycodeval);
    });
    $("#city").keyup(function () {
        var cityval = $('#city').val();
        $(".city1").text(cityval);
    });
    $("#pincode").keyup(function () {
        $('.ss').hide();
        $('.content').show();
        var pincodeval = $('#pincode').val();
        $(".pincode1").text(pincodeval);
    });
    $("#state").keyup(function () {
        var stateval = $('#state').val();
        $(".state1").text(stateval);
    });
    $("#country").change(function () {
        var countryval = $(this).children("option:selected").val();
        $(".country1").text(countryval);
    });


    $(".colors").click(function () {
        var price = $("input[name='colors']:checked").val();
        document.getElementById('colors').value = price;
        $(".top-border1").css('background-color', price);
        $(".top-border").css('background-color', price);
        $(".abc").css('background-color', price);
        $(".share").css('background-color', price);

        $("#abbtn").css('background-color', price);
        $("#abbtn1").css('background-color', price);
        $("#abbtn2").css('background-color', price);
        $("#cbtn").css('background-color', price);
        $("#cbtn1").css('background-color', price);
        $("#cbtn2").css('background-color', price);
        $("#btnpdf2").css('background-color', price);
        $(".top-right-info-btm li").css('border', price + ' solid');
        $(".top-right-info-btm span").css('color', price);
        $(".picon").css('color', price);
        $(".copy_right p").css('background-color', price);
        $(".card-header").css('background-color', price);


    });

    // ====================================//CUSTOM JS==============-====

    $("#countrycode").change(function () {
        var contry = $('#countrycode').val();
        $(".contrycodetext").text('+(' + contry + ')');
    });

    //   $("#mobile").keyup(function(){
    //    var mobile = $('#mobile').val();
    // $(".mobiletext").text(mobile);
    //  });

    $("#maplink").keyup(function () {
        var map = $('#maplink').val();
        if (map != '') {
            $(".mapli").show();
            $(".maplinkk").attr("href", map);
        }
        else {
            $(".mapli").hide();
        }
    });

    $("input[name=logor]").change(function () {
        var ll = $("input[name=logor]:checked").val();
        if (ll == '1') {
            $("#himg").css("width", "100%");
            $("#himg").css("height", "300px");
            $("#himg1").css("width", "80%");
            $("#himg1").css("height", "250px");
            $("#himg2").css("width", "80%");
            $("#himg2").css("height", "250px");
        }
        else {
            $("#himg").css("width", "100%");
            $("#himg").css("height", "auto");
            $("#himg1").css("width", "80%");
            $("#himg1").css("height", "auto");
            $("#himg2").css("width", "80%");
            $("#himg2").css("height", "auto");
        }

    });

    $("#colors").change(function () {
        var price = $('#colors').val();
        $(".abc").css('background-color', price);
        $(".top-border1").css('background-color', price);
        $(".share").css('background-color', price);
        $(".top-border").css('background-color', price);
        $("#abbtn").css('background-color', price);
        $("#abbtn1").css('background-color', price);
        $("#abbtn2").css('background-color', price);
        $("#cbtn").css('background-color', price);
        $("#cbtn1").css('background-color', price);
        $("#cbtn2").css('background-color', price);
        $("#btnpdf2").css('background-color', price);
        $(".top-right-info-btm li").css('border', price + ' solid');
        $(".top-right-info-btm span").css('color', price);
        $(".picon").css('color', price);
        $(".copy_right p").css('background-color', price);
        $(".card-header").css('background-color', price);


    });
    $("#tcolors").change(function () {
        var tprice = $('#tcolors').val();
        $('.bh4').css('color', tprice);

    });

    $("#logo").change(function () {
        $('.ss').hide();
        $('.content').show();
        var image = document.getElementById('himg');
        var image1 = document.getElementById('himg1');
        var image2 = document.getElementById('himg2');
        image.src = URL.createObjectURL(event.target.files[0]);
        image1.src = URL.createObjectURL(event.target.files[0]);
        image2.src = URL.createObjectURL(event.target.files[0]);
        $("#himg1").css("width", "100%");
        $("#himg1").css("height", "auto");
    });

});
