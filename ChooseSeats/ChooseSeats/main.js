/// <reference path="scripts/jquery-2.2.4.js" />

(function () {
    $(function () {
        let saloons = [
            { name: "سالن۱", value: [20, 20, 20, 20, 15, 15, 10] },
            { name: "سالن۲", value: [40, 40, 35, 35, 35, 30, 25, 25, 20, 20] },
            { name: "سالن۳", value: [20, 20, 15, 10, 10] },
        ]

        function RegisteredSeats(name, family, seat) {
            this.firstName = name;
            this.lastName = family;
            this.seatNo = seat;
        }

        var selectedSeats = [];

        let $wrapper = $("<wrapper>");
        $wrapper.css("text-align", "center");

        for (let i in saloons) {
            $("<option>").val(i).html(saloons[i].name).appendTo("select");
        }

        $("select").change(() => {
            $wrapper.html("");
            let selectedSaloon = $("select").val();
            let saloonSeats = saloons[selectedSaloon].value;

            for (let ix in saloonSeats) {
                var $seatButtons = $("<div>").addClass("col-md-12");
                for (let i = 0; i < saloonSeats[ix]; i++) {
                    $seatButtons.append(
                        $("<button>")
                            .addClass("btn btn-success btn-xs")
                            .text(i + 1)
                            .click(function () {
                                selectedSeats.push(+$(this).html());
                                $(this).toggleClass('btn-success').toggleClass('btn-warning');
                            })
                    )
                }
                $wrapper.append($seatButtons).appendTo("main");
            }
        })
        $("#submit").click(function () {
           
                let $name = $("#name").val();
                let $family = $("#familyname").val();
                let seat = selectedSeats[0];

                $.ajax({
                    method: "GET",
                    url: "/dataget.aspx",
                    data: { name: $name, family: $family, seat: seat }
                })
            
            });
    
    
    });
})();