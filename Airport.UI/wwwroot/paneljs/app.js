
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
    console.log("asdasd")
    if ($(this).find("input").prop("checked")) {
        $(this).find("input").prop("checked", false)
        $(this).addClass("active")
    }
    else {
        $(this).find("input").prop("checked", true)
        $(this).removeClass("active")
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
            required: "This Field is Required",
        },
        Surname: {
            required: "This Field is Required",
        },
        Email: {
            required: "This Field is Required",
        },
        Phone: {
            required: "This Field is Required",
        },
    },
    submitHandler: function (form) {

        $("body").append(`
        <div class="createLoader active">
            <p>Creating</p>
            <div class="words">
                <span class="word">Reservation</span>
                <span class="word">PDF</span>
            </div>
        </div>
        `)

        form.submit();
    },
});


$(document).ready(function () {
    // Banner formundaki comboboxlara eklenti eklendi
    //$("._Pick_Up").select2();
    //$("._Drop_Off").select2();
    // Banner formundaki comboboxlara eklenti eklendi

    $(".globalLoader").addClass("deActive");
});