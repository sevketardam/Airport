
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