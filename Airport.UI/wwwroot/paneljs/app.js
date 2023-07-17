

$(document).ready(function () {
    $(".globalLoader").addClass("deActive");
    $(".formLoader").addClass("deActive");
    $("#serviceList").select2();
});

// Araç Ekleme Form Validasyonu
$("#addMyCarForm").validate({
    rules: {
        Brand: {
            required: true,
        },
        Model: {
            required: true,
        },
        Series: {
            required: true,
        },
        Trim: {
            required: true,
        },
        Class: {
            required: true,
        },
        Type: {
            required: true,
        },
        MaxPasseng: {
            required: true,
        },
        SuitCase: {
            required: true,
        },
        SmallBags: {
            required: true,
        },
    },
    messages: {
        Brand: {
            required: "This Field is Required",
        },
        Model: {
            required: "This Field is Required",
        },
        Series: {
            required: "This Field is Required",
        },
        Trim: {
            required: "This Field is Required",
        },
        Class: {
            required: "This Field is Required",
        },
        Type: {
            required: "This Field is Required",
        },
        MaxPasseng: {
            required: "This Field is Required",
        },
        SuitCase: {
            required: "This Field is Required",
        },
        SmallBags: {
            required: "This Field is Required",
        },
    },
    submitHandler: function (form) {
        form.submit();
    },
});


// Step 1 sayfasý Form Validasyonu
$("#location_step1").validate({
    rules: {
        place_Id: {
            required: true,
        }
    },
    messages: {
        place_Id: {
            required: "This Field is Required",
        },
    },
    
    submitHandler: function (form) {
        form.submit();
    },
});


// Sürücü Ekleme Form Validasyonu
$("#addDriverForm").validate({
    rules: {
        Name: {
            required: true,
        },
        Email: {
            required: true,
            email:true
        },
        Password: {
            required: true,
        },
        Phone: {
            required: true,
        },
    },
    messages: {
        Name: {
            required: "This Field is Required",
        },
        Email: {
            required: "This Field is Required",
        },
        Password: {
            required: "This Field is Required",
        },
        Phone: {
            required: "This Field is Required",
        },
    },
});

// Sürücü Güncelleme Form Validasyonu
$("#updateDriverForm").validate({
    rules: {
        Name: {
            required: true,
        },
        Email: {
            required: true,
            email: true
        },
        Password: {
            required: true,
        },
        Phone: {
            required: true,
        },
    },
    messages: {
        Name: {
            required: "This Field is Required",
        },
        Email: {
            required: "This Field is Required",
        },
        Password: {
            required: "This Field is Required",
        },
        Phone: {
            required: "This Field is Required",
        },
    },
});

// Servis Ekleme Form Validasyonu
$("#addServiceForm").validate({
    rules: {
        serviceName: {
            required: true,
        },
        serviceItems: {
            required: true,
        },
    },
    messages: {
        serviceName: {
            required: "This Field is Required",
        },
        serviceItems: {
            required: "This Field is Required",
        },
    },
});

// Servis Güncelleme Form Validasyonu
$("#updateServiceForm").validate({
    rules: {
        serviceName: {
            required: true,
        },
        serviceItems: {
            required: true,
        },
    },
    messages: {
        serviceName: {
            required: "This Field is Required",
        },
        serviceItems: {
            required: "This Field is Required",
        },
    },
});



$("#addServiceCategoryForm").validate({
    rules: {
        categoryName: {
            required: true,
        },
    },
    messages: {
        categoryName: {
            required: "This Field is Required",
        },
    },
});


$("#addServicePropertyForm").validate({
    rules: {
        ServiceCategoryId: {
            required: true,
        },
        PropertyName: {
            required: true,
        },
        PropertyDescription: {
            required: true,
        },
    },
    messages: {
        ServiceCategoryId: {
            required: "This Field is Required",
        },
        PropertyName: {
            required: "This Field is Required",
        },
        PropertyDescription: {
            required: "This Field is Required",
        },
    },
});

$(".extrasItem").on("click", function () {
    console.log($(this))
    if ($(this).find("input").prop("checked")) {
        $(this).find("input").prop("checked", false)
        $(this).removeClass("active")
    }
    else {
        $(this).find("input").prop("checked", true)
        $(this).addClass("active")
    }
})


$("#discountCheck").on("change", function () {
    $(this).closest("div").find("p").toggleClass("discountP")
    $("._Total_Flex div.price").toggleClass("active");
})



$("[name='FlightTime']").on("change", function () {
    setTimeout(() => {
        $("#-error").text("!")
        $(`.Return_Arrival [type="datetime-local"]`).prop("min", $(`[name="FlightTime"]`).val())
        $(`.Return_Arrival [type="datetime-local"]`).prop("value ", $(`[name="FlightTime"]`).val())
    }, 1);
})

$(".Return_Arrival input[type='datetime-local']").on("change", function () {
    setTimeout(() => {
        $("#-error").text("!")
    }, 1);
})

$("#_Search_Booking").on("click", function () {
    setTimeout(() => {
        $("#-error").text("!")
    }, 1);
})



$("._Add_Return, .Roundtrip_Btn").on("click", function () {
    $("._Add_Return").closest("div").attr("class", "d-none _DeActive")
    $(".Return_Arrival").attr("class", "Return_Arrival")
    $(`[name="ReturnStatus"]`).prop("checked", true);
    $(`.Return_Arrival [type="datetime-local"]`).prop("min", $(`[name="FlightTime"]`).val())
})


$(".One_Way").on("click", function () {
    $("._Add_Return").closest("div").attr("class", "")
    $(".Return_Arrival").attr("class", "d-none Return_Arrival")
    $(`[name="ReturnStatus"]`).prop("checked", false);
})



$("#_Passenger_Form").validate({
    rules: {
        Name: {
            required: true,
        },
        Surname: {
            required: true,
        },
        Email: {
            required: true,
            email: true,
        },
        Phone: {
            required: true,
        },
    },
    messages: {
        Name: {
            required: "!",
        },
        Surname: {
            required: "!",
        },
        Email: {
            required: "!",
            email: "!",
        },
        Phone: {
            required: "!",
        },
    },
    //submitHandler: function (form) {

    //    $("body").append(`
    //    <div class="createLoader active">
    //        <p>Creating</p>
    //        <div class="words">
    //            <span class="word">Reservation</span>
    //            <span class="word">Voucher</span>
    //        </div>
    //    </div>
    //    `)
    //    $("html, body").css("overflow", "hidden");

    //    form.submit();
    //},
});

//$("body").on("click", ".pageServiceDownItemTop", function (e) {
//    if ($(e.target).attr("class") != "passengerInputs" && $(e.target).attr("class") != "passengerInputs valid") {
//        $(this).closest(".pageServiceDownItem").find(".pageServiceDownItemBottom").slideToggle(250);
//        $(this).closest(".pageServiceDownItem").find(".pageServiceAngle").toggleClass("active");
//    }
//})

$("._Dash_Right button").on("click", function () {
    location.pathname = `${$(this).attr("href") }`;
})

$("._Center_History_Area .dateBtn").on("click", function () {
    let filter = $(this).attr("date-js");
    let control = 0;
    $("._Bottom_Table .tableDatas > div").each((key, data) => {

        if ($(data).attr("date-js") == filter) {
            $(data).show();
        }
        else {
            $(data).hide();
            control++;
        }

        if ($("._Bottom_Table .tableDatas > div").length == control) {
            $("._Bottom_Table .tableDatas").append(`
                <div class="_No_Data">
                    No Data
                </div>
            `)
            control = 0;
        }
        else {
            $("._Bottom_Table .tableDatas _No_Data").remove();
        }

        


    })
})

$(".step2_top_infos button").click(function () {
    $(this).toggleClass("active");
    $(".top_infos_details").slideToggle(250)
})

$(".hidden-link a").on("click", function () {
    $(this).toggleClass("active");
    $(".step3-extra-details").slideToggle(300);
    setTimeout(() => {
        if ($(this).hasClass("active")) {
            $(this).text("Hide Details");
        }
        else {
            $(this).text("Show Details");
        }
    }, 10);
})


$(".doc-item input").on("change", function () {
    $(this).closest(".doc-item").find("p").text($(this).val().split(`\\`)[$(this).val().split(`\\`).length - 1])
})

$("body").on("click", ".pageServiceDownItemTop", function (e) {
    console.log($(e.target).attr("class"))
    if ($(e.target).attr("class") != "peopleCountText" &&
        $(e.target).attr("class") != "peopleCountText text-center" &&
        $(e.target).attr("class") != "peopleCountText text-center text-block" &&
        $(e.target).attr("class") != "negative_icon" &&
        $(e.target).attr("class") != "fa-solid fa-minus" &&
        $(e.target).attr("class") != "positive_icon" &&
        $(e.target).attr("class") != "fa-solid fa-plus") {
        $(this).closest(".pageServiceDownItem").find(".pageServiceDownItemBottom").slideToggle(250);
        $(this).closest(".pageServiceDownItem").find(".pageServiceAngle").toggleClass("active");
    }
})