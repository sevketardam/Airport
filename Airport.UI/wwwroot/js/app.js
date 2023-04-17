$("._Auth ul._Btns > li._Active > a").on("click", function () {
    $("._Sing_Form_Main").toggleClass("_Active");
  });
  
  $(document).ready(function () {
    $("._Pick_Up").select2(); // Banner formundaki comboboxlara eklenti eklendi
    $("._Drop_Off").select2(); // Banner formundaki comboboxlara eklenti eklendi
  });
  
  // Banner formunda comboboxların value değerlerini almak için
  $("._Pick_Up").on("change", function () {
    var selectedValue = $(this).val();
    console.log(selectedValue);
  });
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
      user_email: {
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
  