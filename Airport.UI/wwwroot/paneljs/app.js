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
        Service: {
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
        Service: {
            required: "This Field is Required",
        },
    },
    submitHandler: function (form) {
        form.submit();
    },
});






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

//$("#updateMyCarForm").validate({
//    rules: {
//        Brand: {
//            required: true,
//        },
//        Model: {
//            required: true,
//        },
//        Series: {
//            required: true,
//        },
//        Trim: {
//            required: true,
//        },
//        Class: {
//            required: true,
//        },
//        Type: {
//            required: true,
//        },
//        MaxPasseng: {
//            required: true,
//        },
//        SuitCase: {
//            required: true,
//        },
//        SmallBags: {
//            required: true,
//        },
//        Service: {
//            required: true,
//        },
//    },
//    messages: {
//        Brand: {
//            required: "This Field is Required",
//        },
//        Model: {
//            required: "This Field is Required",
//        },
//        Series: {
//            required: "This Field is Required",
//        },
//        Trim: {
//            required: "This Field is Required",
//        },
//        Class: {
//            required: "This Field is Required",
//        },
//        Type: {
//            required: "This Field is Required",
//        },
//        MaxPasseng: {
//            required: "This Field is Required",
//        },
//        SuitCase: {
//            required: "This Field is Required",
//        },
//        SmallBags: {
//            required: "This Field is Required",
//        },
//        Service: {
//            required: "This Field is Required",
//        },
//    },
//    submitHandler: function (form) {
//        form.submit();
//    },
//});



