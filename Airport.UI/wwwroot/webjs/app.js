$("._Auth ul._Btns > li._Active > a").on("click", function () {
    $("._Sing_Form_Main").toggleClass("_Active");
});

$(document).ready(function () {
    // Banner formundaki comboboxlara eklenti eklendi
    $("._Pick_Up").select2();
    $("._Drop_Off").select2();
    // Banner formundaki comboboxlara eklenti eklendi
});

// Banner formunda comboboxların value değerlerini almak için
$("._Pick_Up").on("change", function () {
    var selectedValue = $(this).val();
    console.log(selectedValue);
});
// Banner formunda comboboxların value değerlerini almak için
$("._Drop_Off").on("change", function () {
    var selectedValue = $(this).val();
    console.log(selectedValue);
});

$(".menuToggle").on("click", function () {
    $("._Nav_Content").toggleClass("_Active");
});

// Banner Form Validation
$('form[id="_Sing_Form"]').validate({
    rules: {
        mail: {
            required: true,
            email: true,
        },
        password: {
            required: true,
            minlength: 6,
        },
    },
    messages: {
        mail: {
            email: "Enter a valid email",
        },
        psword: {
            minlength: "Password must be at least 6 characters long",
        },
    },
    submitHandler: function (form) {
        form.submit();
    },
});

// Register sayfasındaki formun validasyonu
$('form[id="_Register_Form"]').validate({
    rules: {
        name: {
            required: true,
        },
        eposta: {
            required: true,
            email: true,
        },
        phoneNumber: {
            required: true,
        },
        password: {
            required: true,
            minlength: 6,
        },
        c1: {
            required: true,
        },
        c2: {
            required: true,
        },
    },
    messages: {
        name: {
            required: "This Field is Required",
        },
        eposta: {
            email: "Enter a valid email",
        },
        phoneNumber: {
            minlength: "Password must be at least 6 characters long",
        },
        password: {
            minlength: "Password must be at least 6 characters long",
        },
        c1: {
            required: "Contracts Must Be Approved",
        },
        c2: {
            required: "Contracts Must Be Approved",
        },
    },
    submitHandler: function (form) {
        form.submit();
    },
});

// Agencies sayfasındaki formun validasyonu
$('form[id="_Agency_Form"]').validate({
    rules: {
        Title: {
            required: true,
        },
        E_mail: {
            required: true,
            email: true,
        },
        Name_Surname: {
            required: true,
        },
        Phone_Number: {
            required: true,
        },
        Password: {
            required: true,
            minlength: 6,
        },
        Company_Name: {
            required: true,
        },
        Adress: {
            required: true,
        },
        Transfer_Request: {
            required: true,
        },
        Transfer_Requested_Locations: {
            required: true,
        },
        Company_Phone_Number: {
            required: true,
        },
        Tell_Phone_Number: {
            required: true,
        },
        file: {
            required: true,
        },
        c1: {
            required: true,
        },
        c2: {
            required: true,
        },
    },
    messages: {
        E_mail: {
            email: "Enter a valid email",
        },
        Password: {
            minlength: "Password must be at least 6 characters long",
        },
        c1: {
            required: "Contracts Must Be Approved",
        },
        c2: {
            required: "Contracts Must Be Approved",
        },
    },
    submitHandler: function (form) {
        form.submit();
    },
});

// İletişim sayfasındaki formun validasyonu
$('form[id="_Contact_Form"]').validate({
    rules: {
        mail: {
            required: true,
            email: true,
        },
        message: {
            required: true,
        },
    },
    messages: {
        mail: {
            email: "Enter a valid email",
        },
    },
    submitHandler: function (form) {
        form.submit();
    },
});

// Klavyeden sadece rakam girişlerine izin veren function
function numControl(event) {
    if (
        event.keyCode == 46 ||
        event.keyCode == 8 ||
        event.keyCode == 9 ||
        event.keyCode == 27 ||
        event.keyCode == 13 ||
        (event.keyCode == 65 && event.ctrlKey === true) ||
        (event.keyCode >= 35 && event.keyCode <= 39) ||
        (event.keyCode >= 96 && event.keyCode <= 105)
    ) {
        return true;
    }
    if (event.keyCode < 48 || event.keyCode > 57) {
        event.preventDefault();
        return false;
    }
    return true;
}

// Anasayfadaki araçların slider kodu
var swiper = new Swiper("._Car_Slider", {
    slidesPerView: 1,
    spaceBetween: 10,

    navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
    },
    breakpoints: {
        480: {
            slidesPerView: 1,
            spaceBetween: 0,
        },
        576: {
            slidesPerView: 2,
            spaceBetween: 10,
        },
        768: {
            slidesPerView: 3,
            spaceBetween: 20,
        },
        992: {
            slidesPerView: 4,
            spaceBetween: 30,
        },
        1024: {
            slidesPerView: 5,
            spaceBetween: 40,
        },
    },
});

// Browse Butonuna basıldığında file inputun click olması sağlandı
$("._Clone_Input").on("click", function () {
    $("._File").click();
});

// file inputun value değerini alır
$("._File").on("change", function () {
    console.log($(this).val());
});

$(".active_lang").on("click", function () {
    $(this).toggleClass("active")
})



$('._Reservatiton_Style form').validate({
    rules: {
        reservationCode: {
            required: true,
        },
        email: {
            required: true,
            email: true,
        }
    },
    messages: {
        reservationCode: {
            required: "This Field is Required",
            email: "Invalid Email"
        },
        email: {
            required: "This Field is Required"
        }
    },
    submitHandler: function (form) {
        form.submit();
    },
});

$('form#_Sign_Form').validate({
    rules: {
        UserEposta: {
            required: true,
            email: true,
        },
        UserPassword: {
            required: true,
        }

    },
    messages: {
        UserEposta: {
            required: "This Field is Required"
        },
        UserPassword: {
            required: "This Field is Required",
        },
    },
});







//, .Return_Arrival[type = 'datetime-local']
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
    $(`[name="return-controle"]`).prop("checked", true);
    $(`.Return_Arrival [type="datetime-local"]`).prop("min", $(`[name="FlightTime"]`).val())
})


$(".One_Way").on("click", function () {
    $("._Add_Return").closest("div").attr("class", "")
    $(".Return_Arrival").attr("class", "d-none Return_Arrival")
    $(`[name="return-controle"]`).prop("checked", false);
})
