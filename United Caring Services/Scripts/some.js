function fbfd(x,y) {
    var url = "https://graph.facebook.com/v2.12/UnitedCaringServices/" + y;

    var posts = [];
    $.ajax({
        method: 'GET',
        url: url,
        headers: { 'Authorization': x },
        crossdomain: true,
        datatype: 'json',
        async: false,
        success: function (mydata) {
            for (i = 0; i < mydata.data.length; i++) {

                if (y == "posts") {
                    if (mydata.data[i].message != null) {
                        var year = mydata.data[i].created_time.slice(0, 4);
                        var month = mydata.data[i].created_time.slice(5, 7);
                        var day = mydata.data[i].created_time.slice(8, 10);

                        $('#fbticker').append(
                            '<li>' + month + '/' + day + '/' + year + ' <a>' + mydata.data[i].message + '</a></li>'

                        );
                    };

                };

                if (y == 'events') {
                    var startyear = mydata.data[i].start_time.slice(0, 4);
                    var startmonth = mydata.data[i].start_time.slice(5, 7);
                    var startday = mydata.data[i].start_time.slice(8, 10);
                    var startTime = mydata.data[i].start_time.slice(11, 16);
                    var endyear = mydata.data[i].end_time.slice(0, 4);
                    var endmonth = mydata.data[i].end_time.slice(5, 7);
                    var endday = mydata.data[i].end_time.slice(8, 10);
                    var endTime = mydata.data[i].end_time.slice(11, 16);
                    var formattedStartTime = tConvert(startTime);
                    var formattedEndTime = tConvert(endTime);
                    var startDate = startmonth + '/' + startday + '/' + startyear + ' ' + formattedStartTime;
                    var endDate = endmonth + '/' + endday + '/' + endyear + ' ' + formattedEndTime;
                    var desc;
                    if (startday == endday && startmonth == endmonth && startyear == endyear) {
                        endDate = formattedEndTime;
                    }


                    if (mydata.data[i].description.length >= 5000) {
                        desc = mydata.data[i].description.slice(0, 500) + '...';
                    } else {
                        desc = mydata.data[i].description;
                    }

                    var td = new Date();
                    var day = td.getDate();
                    var month = td.getMonth();
                    var year = td.getFullYear();

                    if (parseInt(startyear) >= parseInt(year)) {
                        if (parseInt(startmonth) >= parseInt(month)) {
                                $('#eventsTable').append(
                                    '<h3><a href="https://www.facebook.com/events/' + mydata.data[i].id + '">' + mydata.data[i].name +
                                    '</a></h3>' +
                                    '<li>' + desc +
                                    '</li>' +
                                    '<li>' + mydata.data[i].place.name + ' ' + mydata.data[i].place.location.city + ', ' + mydata.data[i].place.location.state +
                                    '</li>' +
                                    '<li>' + startDate + '-' + endDate +
                                    '</li>' +
                                    '<hr />'
                                );
                        } else if ((parseInt(startmonth) == parseInt(month)) && (parseInt(startday) >= parseInt(day))) {
                            $('#eventsTable').append(
                                '<a href="https://www.facebook.com/events/' + mydata.data[i].id + '">' + mydata.data[i].name +
                                '</a>' +
                                '<li>' + desc +
                                '</li>' +
                                '<li>' + mydata.data[i].place.name + ' ' + mydata.data[i].place.location.city + ', ' + mydata.data[i].place.location.state +
                                '</li>' +
                                '<li>' + startDate + '-' + endDate +
                                '</li>' +
                                '<hr />'
                            );
                        }
                    }

                }
            };
        },
        error: function (status) {
            console.log(status)
        }


    })
}

$.fn.liScroll = function (settings) {
    settings = jQuery.extend({
        travelocity: 0.03
    }, settings);
    return this.each(function () {
        var $strip = jQuery(this);
        $strip.addClass("newsticker")
        var stripHeight = 1;
        $strip.find("li").each(function (i) {
            stripHeight += jQuery(this, i).outerHeight(true); // thanks to Michael Haszprunar and Fabien Volpi
        });
        var $mask = $strip.wrap("<div class='mask'></div>");
        var $tickercontainer = $strip.parent().wrap("<div class='tickercontainer'></div>");
        var containerHeight = $strip.parent().parent().height();	//a.k.a. 'mask' width 	
        $strip.height(stripHeight);
        var totalTravel = stripHeight;
        var defTiming = totalTravel / settings.travelocity;	// thanks to Scott Waye		
        function scrollnews(spazio, tempo) {
            $strip.animate({ top: '-=' + spazio }, tempo, "linear", function () { $strip.css("top", containerHeight); scrollnews(totalTravel, defTiming); });
        }
        scrollnews(totalTravel, defTiming);
        $strip.hover(function () {
            jQuery(this).stop();
        },
            function () {
                var offset = jQuery(this).offset();
                var residualSpace = offset.top + stripHeight;
                var residualTime = residualSpace / settings.travelocity;
                scrollnews(residualSpace, residualTime);
            });
    });
};

$(function () {
    $("ul#fbticker").liScroll();
});


function tConvert(time) {
    // Check correct time format and split into components
    time = time.match(/^([01]\d|2[0-3])(:)([0-5]\d)(:[0-5]\d)?$/) || [time];

    if (time.length > 1) { // If time format correct
        time = time.slice(1);  // Remove full string match value
        time[5] = +time[0] < 12 ? 'AM' : 'PM'; // Set AM/PM
        time[0] = +time[0] % 12 || 12; // Adjust hours
    }
    return time.join(''); // return adjusted time or original string
}


