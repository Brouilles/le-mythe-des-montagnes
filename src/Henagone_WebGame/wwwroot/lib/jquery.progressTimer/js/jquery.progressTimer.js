(function ($) {
    $.fn.progressTimer = function (options) {
		var settings = $.extend({}, $.fn.progressTimer.defaults, options);

		this.each(function () {
		    var t = $(this);
		    t.empty();
            var barContainer = $("<div>").addClass("progress");
            var bar = $("<div>").addClass("progress-bar").addClass(settings.baseStyle)
                .attr("role", "progressbar")
                .attr("aria-valuenow", "0")
                .attr("aria-valuemin", "0")
                .attr("aria-valuemax", settings.timeLimit);

            bar.appendTo(barContainer);
            barContainer.appendTo($(this));
            
            var start;
            if (settings.start === 0) {
                start = new Date();
            }
            else {
                start = settings.start
            }

            var limit = settings.timeLimit * 1000;
            var interval = window.setInterval(function () {
                settings.allTheTime.call(this, settings);
                limit = settings.timeLimit * 1000;

                var elapsed = new Date() - start;
                if (!t.children().hasClass('progress')) {
                    window.clearInterval(interval);
                }
                
                if (limit - elapsed <= 5000)
                    bar.removeClass(settings.baseStyle)
                       .removeClass(settings.completeStyle)
                       .addClass(settings.warningStyle);

                if (elapsed >= limit && parseFloat($(bar)[0].style.width) >= settings.extra) {
                    window.clearInterval(interval);

                    bar.removeClass(settings.baseStyle)
                       .removeClass(settings.warningStyle)
                       .addClass(settings.completeStyle);

                    settings.onFinish.call(this);
                }
                bar.width(((elapsed / limit) * 100) + "%");
                settings.eachIteration.call(this, elapsed);
            }, 250);

        });

        return this;
    };

    $.fn.progressTimer.defaults = {
        timeLimit: 60,  //total number of seconds
        start: 0,
        extra: 100,
        warningThreshold: 5,  //seconds remaining triggering switch to warning color
        eachIteration: function (elapsed) {},
        allTheTime: function (elapsed) {},
        onFinish: function () {},  //invoked once the timer expires
		baseStyle: '',  //bootstrap progress bar style at the beginning of the timer
        warningStyle: 'progress-bar-danger',  //bootstrap progress bar style in the warning phase
        completeStyle: 'progress-bar-success'  //bootstrap progress bar style at completion of timer
    };
}(jQuery));
