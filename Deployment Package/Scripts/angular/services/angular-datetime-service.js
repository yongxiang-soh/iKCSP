angular.module("AngularDateTimeService", [])
    .service("ngDateTime", [
        function ($http) {

            // Date regular expression which is returned by SQL.
            this.sqlDateRegexCheck = /[/]Date[(]([0-9]|[-][0-9])+[)][/]/;

            // Regex which is used for parsing millisecs from sql date.
            this.sqlDateRegexReplace = /[^0-9.]/gi;

            // Find javscript date from sql date.
            this.sqlDateToDate = function (sqlDate) {

                if (!this.sqlDateRegexCheck.test(sqlDate))
                    return null;

                var milliseconds = sqlDate.replace(this.sqlDateRegexReplace, "");
                try {

                    // From the number sequence, parse and initialize Date instance.
                    return new Date(parseInt(milliseconds));
                } catch (exception) {
                    return null;
                }
            }
        }]);