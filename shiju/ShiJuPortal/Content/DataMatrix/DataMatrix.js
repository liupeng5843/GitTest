/// <reference path="../../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../../Scripts/typings/jqueryui/jqueryui.d.ts"/>
var DataMatrixProperty = (function () {
    function DataMatrixProperty() {
    }
    return DataMatrixProperty;
})();
var DataMatrixFooterBarButton = (function () {
    function DataMatrixFooterBarButton() {
    }
    return DataMatrixFooterBarButton;
})();
var DataMatrixRowActionButtonType;
(function (DataMatrixRowActionButtonType) {
    DataMatrixRowActionButtonType[DataMatrixRowActionButtonType["Button"] = 0] = "Button";
    DataMatrixRowActionButtonType[DataMatrixRowActionButtonType["Link"] = 1] = "Link";
})(DataMatrixRowActionButtonType || (DataMatrixRowActionButtonType = {}));
var DataMatrixRowActionButton = (function () {
    function DataMatrixRowActionButton() {
    }
    return DataMatrixRowActionButton;
})();
var DataMatrixOptions = (function () {
    function DataMatrixOptions() {
    }
    return DataMatrixOptions;
})();
var DataMatrixStatic = (function () {
    function DataMatrixStatic() {
    }
    return DataMatrixStatic;
})();
DataMatrixStatic.propertyTypePlugins = {};
$.widget("ui.dataMatrix", {
    _jqgrid: null,
    _bootgrid: null,
    options: new DataMatrixOptions(),
    _create: function () {
        this.options = $.extend(true, {
            implementationType: "jqGrid",
            isMultipleSelection: false,
            isRowNumbersVisible: true,
            isSearchButtonVisible: true,
            isEditButtonVisible: true,
            isCreateButtonVisible: true,
            isDeleteButtonVisible: true,
            isColumnChooserEnabled: false,
            rowCountPerPage: 25,
            rowCountsPerPage: [],
            footerBarButtons: [],
            isHeaderBarHidden: true,
            isFooterBarHidden: true,
        }, this.options || {});
        if (this.element.length > 0) {
            var elem = this.element[0];
            var id = elem.id || ("randomId" + Math.random().toString());
            elem.id = id;
            this.options.elementId = id;
            this.options.idPrefix = id;
            switch (this.options.implementationType.toLowerCase()) {
                case "bootgrid":
                    this._bootgrid = DataMatrixBootgridImplementation.generateGrid(this.options);
                    break;
                default:
                    this._jqgrid = DataMatrixJqGridImplementation.generateGrid(this.options);
                    break;
            }
        }
    },
    _setOption: function (key, value) {
        this.options[key] = value;
    },
    _setBootgridOption: function (key, value) {
        if (typeof this.options.bootgridOptions !== "undefined") {
            this.options.bootgridOptions[key] = value;
        }
    },
    getSelectedRowId: function () {
        var ids = DataMatrixJqGridImplementation.getSelectedRowIds(this._jqgrid, this.options);
        if (ids.length > 0) {
            return ids[0];
        }
        return null;
    },
    getSelectedRowIds: function () {
        return DataMatrixJqGridImplementation.getSelectedRowIds(this._jqgrid, this.options);
    },
    getRow: function (rowId) {
        switch (this.options.implementationType.toLowerCase()) {
            case "bootgrid":
                return DataMatrixBootgridImplementation.getRow(this._bootgrid, rowId);
            default:
                return DataMatrixJqGridImplementation.getRow(this._jqgrid, rowId);
        }
    },
    deleteRow: function (rowId, needConfirmation) {
        switch (this.options.implementationType.toLowerCase()) {
            case "bootgrid":
                return DataMatrixBootgridImplementation.deleteRow(this._bootgrid, this.options, rowId, needConfirmation);
            default:
                return DataMatrixJqGridImplementation.deleteRow(this._jqgrid, this.options, rowId, needConfirmation);
        }
    },
    deleteRows: function (rowIds, needConfirmation) {
        return DataMatrixJqGridImplementation.deleteRow(this._jqgrid, this.options, rowIds, needConfirmation);
    },
    editRow: function (rowId) {
        switch (this.options.implementationType.toLowerCase()) {
            case "bootgrid":
                return DataMatrixBootgridImplementation.editRow(this._bootgrid, this.options, rowId);
            default:
                return DataMatrixJqGridImplementation.editRow(this._jqgrid, this.options, rowId);
        }
    },
    createRow: function () {
        switch (this.options.implementationType.toLowerCase()) {
            case "bootgrid":
                return DataMatrixBootgridImplementation.createRow(this._bootgrid, this.options);
            default:
                return DataMatrixJqGridImplementation.createRow(this._jqgrid, this.options);
        }
    },
    showSearchDialog: function () {
        return DataMatrixJqGridImplementation.showSearchDialog(this._jqgrid, this.options);
    },
    reloadGrid: function () {
        switch (this.options.implementationType.toLowerCase()) {
            case "bootgrid":
                return DataMatrixBootgridImplementation.reload(this._bootgrid);
            default:
        }
    }
});
var DataMatrixJqGridImplementation;
(function (DataMatrixJqGridImplementation) {
    var dateFormat = function () {
        var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g, timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g, timezoneClip = /[^-+\dA-Z]/g, pad = function (val, len) {
            if (len === void 0) { len = 2; }
            val = String(val);
            while (val.length < len)
                val = "0" + val;
            return val;
        };
        // Some common format strings
        var masks = {
            "default": "ddd mmm dd yyyy HH:MM:ss",
            shortDate: "m/d/yy",
            mediumDate: "mmm d, yyyy",
            longDate: "mmmm d, yyyy",
            fullDate: "dddd, mmmm d, yyyy",
            shortTime: "h:MM TT",
            mediumTime: "h:MM:ss TT",
            longTime: "h:MM:ss TT Z",
            isoDate: "yyyy-mm-dd",
            isoTime: "HH:MM:ss",
            isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
            isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
        };
        // Internationalization strings
        var i18n = {
            dayNames: [
                "Sun",
                "Mon",
                "Tue",
                "Wed",
                "Thu",
                "Fri",
                "Sat",
                "Sunday",
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday"
            ],
            monthNames: [
                "Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sep",
                "Oct",
                "Nov",
                "Dec",
                "January",
                "February",
                "March",
                "April",
                "May",
                "June",
                "July",
                "August",
                "September",
                "October",
                "November",
                "December"
            ]
        };
        // Regexes and supporting functions are cached through closure
        return function (date, mask, utc) {
            if (utc === void 0) { utc = false; }
            var dF = dateFormat;
            // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
            if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
                mask = date;
                date = undefined;
            }
            // Passing date through Date applies Date.parse, if necessary
            date = date.replace(/-/g, '/').replace('T', ' ');
            var dotIndex = date.indexOf('.');
            if (dotIndex > 0) {
                date = date.substring(0, dotIndex);
            }
            date = date ? new Date(date) : new Date;
            if (isNaN(date))
                throw SyntaxError("invalid date");
            mask = String(masks[mask] || mask || masks["default"]);
            // Allow setting the utc argument via the mask
            if (mask.slice(0, 4) == "UTC:") {
                mask = mask.slice(4);
                utc = true;
            }
            var _ = utc ? "getUTC" : "get", d = date[_ + "Date"](), D = date[_ + "Day"](), m = date[_ + "Month"](), y = date[_ + "FullYear"](), H = date[_ + "Hours"](), M = date[_ + "Minutes"](), s = date[_ + "Seconds"](), L = date[_ + "Milliseconds"](), o = utc ? 0 : date.getTimezoneOffset(), flags = {
                d: d,
                dd: pad(d),
                ddd: i18n.dayNames[D],
                dddd: i18n.dayNames[D + 7],
                m: m + 1,
                mm: pad(m + 1),
                mmm: i18n.monthNames[m],
                mmmm: i18n.monthNames[m + 12],
                yy: String(y).slice(2),
                yyyy: y,
                h: H % 12 || 12,
                hh: pad(H % 12 || 12),
                H: H,
                HH: pad(H),
                M: M,
                MM: pad(M),
                s: s,
                ss: pad(s),
                l: pad(L, 3),
                L: pad(L > 99 ? Math.round(L / 10) : L),
                t: H < 12 ? "a" : "p",
                tt: H < 12 ? "am" : "pm",
                T: H < 12 ? "A" : "P",
                TT: H < 12 ? "AM" : "PM",
                Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
            };
            return mask.replace(token, function ($0) {
                return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
            });
        };
    }();
    function encodingXml(str) {
        str = str.replace(/\&/g, "&amp;");
        str = str.replace(/\"/g, "&quot;");
        str = str.replace(/\</g, "&lt;");
        str = str.replace(/\>/g, "&gt;");
        return str;
    }
    function endsWithString(str, end) {
        return (str.substring(str.length - end.length) === end);
    }
    function startsWithString(str, start) {
        return (str.substr(0, start.length) === start);
    }
    function combineUrl(url1, url2) {
        var url = endsWithString(url1, "/") ? url1 : url1.concat("/");
        return url.concat(startsWithString(url2, "/") ? url2.substring(1) : url2);
    }
    function getKeyPropertyName(properties) {
        var length = properties.length;
        for (var i = 0; i < length; i++) {
            var property = properties[i];
            if (property.isKey === true) {
                return property.name;
            }
        }
        return "";
    }
    // builds the column headers array from the Json we pass to the
    // main grid builder class.
    function getHeaders(properties, options) {
        var headers = $.map(properties, function (property, idx) {
            return property.displayName;
        });
        return headers;
    }
    // builds the jqGrid column definition from the data we pass.  We need
    // to alter for dates that I can think of, and need to discuss other data
    // types to decide what we'd want to do for each (specifically the search options.)
    function getColumDefinitions(properties) {
        return $.map(properties, function (property, idx) {
            var col = {
                editable: ((property.isEditable === undefined) ? true : property.isEditable),
                editrules: { edithidden: ((property.isEditable === undefined) ? true : property.isEditable) },
                name: property.name,
                index: property.name,
                search: property.isSearchable,
                sortable: property.isSortable,
                hidden: property.isColumnHidden,
                hidedlg: property.isColumnForbidden,
                property: property,
            };
            //if (ele.formatter) {
            //    col = $.extend(true, { formatter: ele.formatter }, col || {});
            //}
            if (property.columnWidth && (property.columnWidth != 0)) {
                col = $.extend(true, { width: property.columnWidth }, col || {});
            }
            switch (property.type) {
                case "action":
                    col = $.extend(true, col, {
                        search: false,
                        editable: false,
                        editrules: { edithidden: true },
                        sortable: false,
                        index: "_action",
                        name: "_action",
                    });
                    break;
                case "number":
                    col = $.extend(true, {
                        edittype: 'text',
                        searchoptions: {
                            sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge'],
                            dataInit: function (el) {
                                jQuery(el).val("0");
                            },
                        },
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        }
                    }, col || {});
                    break;
                case "boolean":
                    col = $.extend(true, {
                        edittype: 'checkbox',
                        editoptions: { value: "True:False" },
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        },
                        stype: "select",
                        searchoptions: { sopt: ['eq', 'ne'], value: "true:Yes;false:No" },
                        formatter: function (cellvalue, options, rowObject) {
                            return cellvalue ? "是" : "否";
                        },
                        unformat: function (cellvalue, options, cell) {
                            return (cellvalue === "是") ? "True" : "False";
                        }
                    }, col || {});
                    break;
                case "string":
                    col = $.extend(true, {
                        edittype: 'text',
                        searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge', 'bw', 'bn', 'ew', 'en', 'cn', 'nc', 'nu', 'nn'] },
                        searchrules: {},
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="white-space: normal; vertical-align: middle;"';
                        }
                    }, col || {});
                    break;
                case "multilinestring":
                    col = $.extend(true, {
                        edittype: 'textarea',
                        editoptions: { rows: "5", cols: "dummy" },
                        searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge', 'bw', 'bn', 'ew', 'en', 'cn', 'nc', 'nu', 'nn'] },
                        searchrules: {},
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="white-space: normal; vertical-align: middle;"';
                        }
                    }, col || {});
                    break;
                case "enum":
                    var enumValues = property.typeOptions["enumValues"];
                    if (enumValues === undefined) {
                        throw "enumValues in typeOptions must be defined.";
                    }
                    var enumHtmlValues = property.typeOptions["enumHtmlValues"];
                    col = $.extend(true, {
                        edittype: 'select',
                        editoptions: { value: enumValues },
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        },
                        stype: "select",
                        searchoptions: { sopt: ['eq', 'ne'], value: enumValues },
                        formatter: function (cellvalue, options, rowObject) {
                            if (enumHtmlValues === undefined) {
                                return enumValues[cellvalue];
                            }
                            else {
                                return enumHtmlValues[cellvalue];
                            }
                        },
                    }, col || {});
                    break;
                case "enumstring":
                    var enumValues = property.typeOptions["enumValues"];
                    if (enumValues === undefined) {
                        throw "enumValues in typeOptions must be defined.";
                    }
                    var enumHtmlValues = property.typeOptions["enumHtmlValues"];
                    col = $.extend(true, {
                        edittype: 'select',
                        editoptions: { value: enumValues },
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        },
                        stype: "select",
                        searchoptions: { sopt: ['eq', 'ne'], value: enumValues },
                        formatter: function (cellvalue, options, rowObject) {
                            if (enumHtmlValues === undefined) {
                                return enumValues[cellvalue];
                            }
                            else {
                                return enumHtmlValues[cellvalue];
                            }
                        },
                    }, col || {});
                    break;
                case "enumguid":
                    var enumValues = property.typeOptions["enumValues"];
                    if (enumValues === undefined) {
                        throw "enumValues in typeOptions must be defined.";
                    }
                    var enumHtmlValues = property.typeOptions["enumHtmlValues"];
                    col = $.extend(true, {
                        edittype: 'select',
                        editoptions: { value: enumValues },
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        },
                        stype: "select",
                        searchoptions: { sopt: ['eq', 'ne'], value: enumValues },
                        formatter: function (cellvalue, options, rowObject) {
                            if (enumHtmlValues === undefined) {
                                return enumValues[cellvalue];
                            }
                            else {
                                return enumHtmlValues[cellvalue];
                            }
                        },
                    }, col || {});
                    break;
                case "date":
                    col = $.extend(true, {
                        edittype: 'text',
                        editoptions: {
                            dataInit: function (el) {
                                setTimeout(function () {
                                    var ec = 'DatePicker';
                                    if (typeof (jQuery(el).datepicker) !== 'function') {
                                        alert('JQDatePicker javascript not present on the page. Please, include jquery.jqDatePicker.min.js');
                                    }
                                    jQuery(el).datepicker({
                                        dateFormat: 'yy-mm-dd',
                                        minDate: new Date(1900, 0, 1),
                                        maxDate: new Date(2099, 12, 31),
                                        showOn: 'focus',
                                        showButtonPanel: true,
                                        changeMonth: true,
                                        changeYear: true
                                    });
                                }, 200);
                            }
                        },
                        searchoptions: {
                            sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge'],
                            dataInit: function (el) {
                                jQuery(el).val(dateFormat(new Date(), "yyyy-mm-dd"));
                                setTimeout(function () {
                                    var ec = 'DatePicker';
                                    if (typeof (jQuery(el).datepicker) !== 'function') {
                                        alert('JQDatePicker javascript not present on the page. Please, include jquery.jqDatePicker.min.js');
                                    }
                                    jQuery(el).datepicker({
                                        dateFormat: 'yy-mm-dd',
                                        minDate: new Date(1900, 0, 1),
                                        maxDate: new Date(2099, 12, 31),
                                        showOn: 'focus',
                                        showButtonPanel: true,
                                        changeMonth: true,
                                        changeYear: true
                                    });
                                }, 200);
                            },
                        },
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        },
                        formatter: function (cellvalue, options, rowObject) {
                            var date = new Date(cellvalue);
                            return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
                        }
                    }, col || {});
                    break;
                case "datetime":
                    col = $.extend(true, {
                        edittype: 'text',
                        editoptions: {
                            dataInit: function (el) {
                                setTimeout(function () {
                                    if (typeof (jQuery(el).datetimepicker) !== 'function') {
                                        alert('JQDatePicker javascript not present on the page. Please, include jquery-ui-timepicker-addon.js');
                                    }
                                    jQuery(el).datetimepicker({
                                        // dateFormat: 'yy-mm-dd', timeFormat: 'HH:mm', minDateTime: new Date(1900, 0, 1), maxDateTime: new Date(2099, 12, 31), showOn: 'focus', showButtonPanel: true, changeMonth: true, changeYear: true
                                        dateFormat: 'yy/mm/dd',
                                        timeFormat: 'HH:mm',
                                        minDateTime: new Date(1900, 0, 1),
                                        maxDateTime: new Date(2099, 12, 31),
                                        showOn: 'focus',
                                        showButtonPanel: true,
                                        changeMonth: true,
                                        changeYear: true
                                    });
                                }, 200);
                            }
                        },
                        searchoptions: {
                            sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge'],
                            dataInit: function (el) {
                                //jQuery(el).val(dateFormat(new Date(), "yyyy-mm-dd HH:MM"));
                                jQuery(el).val(dateFormat(new Date(), "yyyy/mm/dd HH:MM"));
                                setTimeout(function () {
                                    if (typeof (jQuery(el).datetimepicker) !== 'function') {
                                        alert('JQDatePicker javascript not present on the page. Please, include jquery-ui-timepicker-addon.js');
                                    }
                                    jQuery(el).datetimepicker({
                                        //dateFormat: 'yy-mm-dd', timeFormat: 'HH:mm', minDateTime: new Date(1900, 0, 1), maxDateTime: new Date(2099, 12, 31), showOn: 'focus', showButtonPanel: true, changeMonth: true, changeYear: true
                                        dateFormat: 'yy/mm/dd',
                                        timeFormat: 'HH:mm',
                                        minDateTime: new Date(1900, 0, 1),
                                        maxDateTime: new Date(2099, 12, 31),
                                        showOn: 'focus',
                                        showButtonPanel: true,
                                        changeMonth: true,
                                        changeYear: true
                                    });
                                }, 200);
                            },
                        },
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        },
                        formatter: function (cellvalue, options, rowObject) {
                            var typeOptions = options.colModel.property.typeOptions;
                            var date;
                            if (cellvalue.indexOf("Z") < 0) {
                                date = new Date(cellvalue + "Z");
                            }
                            if ((typeOptions != null) && (typeOptions.displayInMultipleLines)) {
                                return dateFormat(date, "yyyy-mm-dd") + " <br/>" + dateFormat(date, "HH:MM"); // A space character before <br/> is a must.
                            }
                            return dateFormat(date, "yyyy-mm-dd HH:MM");
                        },
                        unformat: function (cellvalue, options, cell) {
                            var typeOptions = options.colModel.property.typeOptions;
                            if ((typeOptions != null) && (typeOptions.displayInMultipleLines)) {
                                return cellvalue.replace("<br/>", "");
                            }
                            return cellvalue;
                        }
                    }, col || {});
                    break;
                case "imageurl":
                    col = $.extend(true, {
                        edittype: 'text',
                        editrules: { url: true, required: false },
                        searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge', 'bw', 'bn', 'ew', 'en', 'cn', 'nc', 'nu', 'nn'] },
                        searchrules: {},
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        },
                        formatter: function (cellvalue, options, rowObject) {
                            if (cellvalue) {
                                return '<img src="' + cellvalue + '" alt="' + cellvalue + '" onload="if(this.width>this.height) this.width=\'100\'; if(this.width<this.height) this.height=\'100\';" />';
                            }
                            return "";
                        },
                        unformat: function (cellvalue, options, cell) {
                            return $('img', cell).length > 0 ? $('img', cell).attr('src') : "";
                        }
                    }, col || {});
                    break;
                case "imagehub":
                    col = $.extend(true, {
                        edittype: 'custom',
                        editoptions: {
                            custom_element: function (value, options) {
                                var id = "form_input_imagehub_" + options.name;
                                var silverlightId = id + "_sl";
                                var valueElementId = id + "_value";
                                var imageUploadKitUrl = property.typeOptions["imageUploadKitUrl"] || "/ClientBin/ImageUploadKit.xap";
                                var elem = document.createElement("div");
                                $(elem).attr("imagehubid", id);
                                var html = "<object id=\"" + silverlightId + "\" data=\"data:application/x-silverlight-2,\" type=\"application/x-silverlight-2\" width=\"100%\" height=\"100\">" + "<param name=\"source\" value=\"" + imageUploadKitUrl + "\" />" + "<param name=\"onError\" value=\"onSilverlightError\" />" + "<param name=\"background\" value=\"white\" />" + "<param name=\"minRuntimeVersion\" value=\"5.0.61118.0\" />" + "<param name=\"windowless\" value =\"true\" / >" + "<param name=\"initparams\" value=\"ImageServiceUrl=" + property.typeOptions["baseUrl"] + ", AppIdOrName=" + property.typeOptions["appIdOrName"] + ",HtmlValueElementId=" + valueElementId + ",IsMultiple=" + (property.typeOptions["isMultiple"] ? "True" : "False") + "\" />" + "<param name=\"autoUpgrade\" value=\"true\" />" + "<a href=\"http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0\" style=\"text-decoration:none\">" + "<img src=\"http://go.microsoft.com/fwlink/?LinkId=161376\" alt=\"Get Microsoft Silverlight\" style=\"border-style:none\" />" + "</a></object>" + "<iframe id=\"_sl_historyFrame\" style=\"visibility:hidden;height:0px;width:0px;border:0px\"></iframe>" + "<input id=\"" + valueElementId + "\" type=\"hidden\" value=\"" + encodingXml(value) + "\" width=\"600\" />";
                                $(elem).html(html);
                                //setTimeout(function () {
                                //    var sl: any = document.getElementById(silverlightId);
                                //    sl.content.page.RefreshValue();
                                //}, 500);
                                return elem;
                            },
                            custom_value: function (elem, operation, value) {
                                var id = $(elem).attr("imagehubid");
                                if (id === undefined) {
                                    return "";
                                }
                                var silverlightId = id + "_sl";
                                var valueElementId = id + "_value";
                                if (operation === 'get') {
                                    return $("#" + valueElementId).val();
                                }
                                else if (operation === 'set') {
                                    var v = $("#" + valueElementId).val(value);
                                    var sl = document.getElementById(silverlightId);
                                    if (sl.content)
                                        sl.content.page.RefreshValue();
                                    return v;
                                }
                            }
                        },
                        searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge', 'bw', 'bn', 'ew', 'en', 'cn', 'nc', 'nu', 'nn'] },
                        searchrules: {},
                        cellattr: function (rowId, tv, rawObject, cm, rdata) {
                            return 'style="vertical-align: middle;"';
                        },
                        formatter: function (cellvalue, options, rowObject) {
                            var typeOptions = options.colModel.property.typeOptions;
                            var imageId = "";
                            var imageExtension = typeOptions.imageExtension;
                            if (imageExtension == undefined) {
                                imageExtension = "jpg";
                            }
                            if ((cellvalue) && (cellvalue !== "")) {
                                if (typeOptions.isMultiple) {
                                    var imageJsonArray = JSON.parse(cellvalue);
                                    if (imageJsonArray.length > 0) {
                                        imageId = imageJsonArray[0].ImageId;
                                    }
                                }
                                else {
                                    imageId = cellvalue;
                                }
                            }
                            if ((imageId) && (imageId !== "") && (imageId !== "00000000000000000000000000000000") && (imageId !== "00000000-0000-0000-0000-000000000000")) {
                                var url = combineUrl(typeOptions.baseUrl, typeOptions.appIdOrName) + "/" + imageId + "_3_100_100." + imageExtension;
                                var encoded = encodingXml(cellvalue);
                                return '<img src="' + url + '" alt="' + url + '" d="' + encoded + '" + onload="if(this.width>this.height) this.width=\'100\'; if(this.width<this.height) this.height=\'100\';" />';
                            }
                            return "";
                        },
                        unformat: function (cellvalue, options, cell) {
                            if (($('img', cell).length) > 0) {
                                return $('img', cell).attr('d');
                            }
                            else {
                                var typeOptions = options.colModel.property.typeOptions;
                                if (typeOptions.isMultiple) {
                                    return "[]";
                                }
                                else {
                                    return "00000000-0000-0000-0000-000000000000";
                                }
                            }
                        }
                    }, col || {});
                    break;
                default:
                    var plugin = DataMatrixStatic.propertyTypePlugins[property.type];
                    if (plugin !== undefined) {
                        col = $.extend(true, {
                            cellattr: function (rowId, tv, rawObject, cm, rdata) {
                                return 'style="vertical-align: middle;"';
                            },
                            formatter: function (cellvalue, options, rowObject) {
                                return plugin.RenderCellHtml(options.rowId, rowObject, cellvalue, options.colModel.property);
                            },
                        }, col || {});
                    }
                    else {
                        debugger;
                    }
                    break;
            }
            //    case "hidden":
            //        col = $.extend(true, {
            //            sortable: false,
            //            hidden: true,
            //            editable: ele.isEditable,
            //            searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge', 'bw', 'bn', 'ew', 'en', 'cn', 'nc', 'nu', 'nn'], searchhidden: true }
            //        }, col || {});
            //        break;
            //    case "hidden-number":
            //        col = $.extend(true, {
            //            sortable: false,
            //            hidden: true,
            //            editable: ele.isEditable,
            //            searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge'], searchhidden: true }
            //        }, col || {});
            //        break;
            //    case "number":
            //        col = $.extend(true, {
            //            sortable: true,
            //            editable: ele.isEditable,
            //            edittype: 'text',
            //            search: ele.allowSearch,
            //            searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge'] },
            //            cellattr: function (rowId, tv, rawObject, cm, rdata) {
            //                return 'style="vertical-align: middle;"';
            //            }
            //        }, col || {});
            //        break;
            //    case "link":
            //        col = $.extend(true, {
            //            sortable: false,
            //            editable: ele.isEditable,
            //            formatter: ele.linkFormatter,
            //            cellattr: function (rowId, tv, rawObject, cm, rdata) {
            //                return 'style="vertical-align: middle;"';
            //            }
            //        }, col || {});
            //        col.search = false;
            //        break;
            //    case "boolean":
            //        col = $.extend(true, {
            //            sortable: true,
            //            editable: ele.isEditable,
            //            cellattr: function (rowId, tv, rawObject, cm, rdata) {
            //                return 'style="vertical-align: middle;"';
            //            },
            //            stype: "select",
            //            formatter: function (cellvalue, options, rowObject) {
            //                return cellvalue ? "Enabled" : "Disabled";
            //            },
            //            searchoptions: { sopt: ['eq', 'ne'], value: "bool-true:Enabled;bool-false:Disabled" }
            //        }, col || {});
            //        break;
            //    case "list-number":
            //    case "list":
            //        var searchops = { sopt: ["eq"] };
            //        if (ele.searchItems !== undefined) {
            //            searchops = $.extend(true, { value: ele.searchItems }, searchops || {});
            //        };
            //        col = $.extend(true, {
            //            sortable: false,
            //            editable: ele.isEditable,
            //            cellattr: function (rowId, tv, rawObject, cm, rdata) {
            //                return 'style="white-space: normal; vertical-align: middle;"';
            //            },
            //            stype: "select",
            //            searchoptions: searchops
            //        }, col || {});
            //        break;
            //    case "lookup":
            //        var searchOps = { sopt: ["eq"] };
            //        if (ele.searchItems !== undefined) {
            //            searchOps = $.extend(true, { value: ele.searchItems }, searchOps || {});
            //        };
            //        col = $.extend(true, {
            //            sortable: true,
            //            editable: ele.isEditable,
            //            cellattr: function (rowId, tv, rawObject, cm, rdata) {
            //                return 'style="white-space: normal; vertical-align: middle;"';
            //            },
            //            stype: "select",
            //            searchoptions: searchOps
            //        }, col || {});
            //        break;
            //    default:
            //        col = $.extend(true, {
            //            sortable: true,
            //            editable: ele.isEditable,
            //            edittype: 'text',
            //            searchoptions: { sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge', 'bw', 'bn', 'ew', 'en', 'cn', 'nc', 'nu', 'nn'] },
            //            searchrules: {},
            //            cellattr: function (rowId, tv, rawObject, cm, rdata) {
            //                return 'style="white-space: normal; vertical-align: middle;"';
            //            }
            //        }, col || {});
            //        break;
            //}
            return col;
        });
    }
    // comparer should be a value of 'and' or 'or'.
    // quoteFlags = { col: element.fieldName, quoteValue: false, isList: true, baseQuery: element.baseQuery, baseQueryParam: element.baseQueryParam };
    function ODataListExpression(comparer, field, dataItems, filter) {
        var quoteData = $.grep(filter, function (element, idx) {
            return element.col === field;
        });
        var params = $.map(dataItems, function (element, idx) {
            var param = quoteDataVal(element, filter);
            return filter.baseQueryParam.replace("{0}", param);
        });
        var paramstring = params.join(" " + comparer + " ");
        return filter.baseQuery.replace("{0}", paramstring);
    }
    // builds out OData expressions... the condition.
    function ODataExpression(op, field, data, filter) {
        var dataVal = quoteDataVal(data, filter);
        // lists are a unique concern.  with lists, we have to provide an xml/json path
        // for OData to query against.  The best way to handle this is to define the base
        // path query within each Index() page, and use that here with some sort of string.replace
        // or string.format javascript function.
        dataVal = encodeURIComponent(dataVal);
        switch (op) {
            case "cn":
                return "substringof(" + dataVal + ", " + field + ") eq true";
            case "nc":
                return "substringof(" + dataVal + ", " + field + ") eq false";
            case "bw":
                return "startswith(" + field + ", " + dataVal + ") eq true";
            case "bn":
                return "startswith(" + field + ", " + dataVal + ") eq false";
            case "ew":
                return "endswith(" + field + ", " + dataVal + ") eq true";
            case "en":
                return "endswith(" + field + ", " + dataVal + ") eq false";
            case "nu":
                return field + " eq null";
            case "nn":
                return field + " ne null";
            default:
                return field + " " + op + " " + dataVal;
        }
    }
    ;
    /// cols is an array.
    function getQuoteFlags(properties) {
        return $.map(properties, function (property, idx) {
            //sortCols
            var quoteFlags;
            switch (property.type) {
                case "action":
                    break;
                case "number":
                    quoteFlags = { col: property.name, quoteValue: false, isList: false };
                    break;
                case "boolean":
                    quoteFlags = { col: property.name, quoteValue: false, isList: false };
                    break;
                case "string":
                    quoteFlags = { col: property.name, quoteValue: true, isList: false };
                    break;
                case "multilinestring":
                    quoteFlags = { col: property.name, quoteValue: true, isList: false };
                    break;
                case "enum":
                    quoteFlags = { col: property.name, quoteValue: false, isList: false };
                    break;
                case "enumstring":
                    quoteFlags = { col: property.name, quoteValue: true, isList: false };
                    break;
                case "enumguid":
                    quoteFlags = { col: property.name, isList: false, convert: function (v) {
                        return "guid'" + v + "'";
                    } };
                    break;
                case "date":
                    quoteFlags = { col: property.name, isList: false, convert: function (v) {
                        return "DateTime'" + v + "T00:00:00'";
                    } };
                    break;
                case "datetime":
                    quoteFlags = { col: property.name, isList: false, convert: function (v) {
                        var t = dateFormat(new Date(v).addMinutes(-(new Date().getTimezoneOffset())), "yyyy-mm-dd HH:MM:ss");
                        return "DateTime'" + t.replace(" ", "T") + "'";
                    } };
                    break;
                case "imageurl":
                    quoteFlags = { col: property.name, quoteValue: true, isList: false };
                    break;
                case "imagehub":
                    quoteFlags = { col: property.name, quoteValue: true, isList: false };
                    break;
                default:
                    var plugin = DataMatrixStatic.propertyTypePlugins[property.type];
                    if (plugin !== undefined) {
                    }
                    else {
                        debugger;
                    }
                    break;
            }
            ;
            //if (property.sortCols !== undefined) {
            //    quoteFlags = $.extend(true, {
            //        sortCols: property.sortCols
            //    }, quoteFlags || {});
            //};
            return quoteFlags;
        });
    }
    ;
    // primarily used for the bid text code, the idea here is that we
    // can do the sorting for numerics and codes so that all of the 13's
    // are grouped together for the UI.
    function buildColumnSort(gridColCommand, filters) {
        var parts = gridColCommand.split(" ");
        if (parts.length !== 2) {
            throw new Error("Cannot build a sort command without the column name and the direction.");
        }
        var col = parts[0];
        var direction = parts[1];
        if (col === "" || direction === "") {
            throw new Error("We need to know both the column name and the sort direction.");
        }
        var quoteData = $.grep(filters, function (element, idx) {
            return element.col === col;
        });
        if (quoteData.length === 0) {
            // if we don't have a definition for the field, then we can't filter/search for it.
            return "";
        }
        ;
        var singleQuoteData = quoteData[0];
        if (singleQuoteData.sortCols !== undefined) {
            var colSorts = [];
            for (var i = 0; i < singleQuoteData.sortCols.length; i++) {
                colSorts.push(singleQuoteData.sortCols[i] + " " + direction);
            }
            ;
            return colSorts.join(", ");
        }
        return col + " " + direction;
    }
    function quoteDataVal(data, filter) {
        var newData = data;
        if (filter.convert) {
            newData = filter.convert(data);
        }
        else if (filter.quoteValue) {
            return "'" + newData + "'";
        }
        return newData;
    }
    function findQuoteFilter(field, filters) {
        var quoteFilter = $.grep(filters, function (element, idx) {
            return element.col === field;
        });
        if (quoteFilter.length === 0) {
            throw new Error("Cannot find appropriate quote filter for field: " + field);
        }
        ;
        return quoteFilter[0];
    }
    var JqGridGlobal = (function () {
        function JqGridGlobal() {
        }
        JqGridGlobal.jqGridSettings = [];
        return JqGridGlobal;
    })();
    function bindConfiguration(key, options) {
        var p = JqGridGlobal.jqGridSettings[options.idPrefix];
        if (p === null || p === undefined) {
            // do init work here.
            p = $.extend(true, {
                originalOptions: options,
                cols: options.properties,
                keyPropertyName: getKeyPropertyName(options.properties),
                gridTitle: "",
                idPrefix: options.idPrefix,
                containerId: options.elementId,
                exportUrl: "/Export/Excel",
                oDataEndPoint: options.oDataUrl,
                useVirtualScrolling: false,
                gridHeight: (options.height === undefined) ? 500 : undefined,
                autowidth: options.autoWidth,
                height: options.height,
                windowResizeDelay: -1,
                quoteFilters: [],
                container: null,
                form: null,
                filenameField: null,
                oDataField: null,
                filterField: null,
                orderByField: null,
                list: null,
                table: null,
                pager: null,
                onRowSelection: undefined,
                onPageChange: undefined,
                onGridReload: undefined,
                onAllSelection: undefined,
                onSortColumnClick: undefined,
            }, {});
            p.quoteFilters = getQuoteFlags(p.cols);
            // this is the container on the html page where the grid will be rendered.
            p.container = $("#" + p.containerId);
            // this form (and the four elements below) are what allow us to do the server-side rendering of the excel file.
            p.form = document.createElement("form");
            p.form.id = p.idPrefix + "-form";
            p.form.method = "POST";
            p.form.action = p.exportUrl;
            document.body.appendChild(p.form);
            // filename
            p.filenameField = document.createElement("input");
            p.filenameField.type = "hidden";
            p.filenameField.id = p.idPrefix + "-filename";
            p.filenameField.name = "Filename";
            p.filenameField.value = "";
            p.form.appendChild(p.filenameField);
            // ODataUrl
            p.oDataField = document.createElement("input");
            p.oDataField.type = "hidden";
            p.oDataField.id = p.idPrefix + "-ODataUrl";
            p.oDataField.name = "ODataUrl";
            p.oDataField.value = p.oDataEndPoint;
            p.form.appendChild(p.oDataField);
            // Filter
            p.filterField = document.createElement("input");
            p.filterField.type = "hidden";
            p.filterField.id = p.idPrefix + "-Filter";
            p.filterField.name = "Filter";
            p.filterField.value = "";
            p.form.appendChild(p.filterField);
            // OrderBy
            p.orderByField = document.createElement("input");
            p.orderByField.type = "hidden";
            p.orderByField.id = p.idPrefix + "-OrderBy";
            p.orderByField.name = "OrderBy";
            p.orderByField.value = "";
            p.form.appendChild(p.orderByField);
            // RequestPage
            p.RequestPageField = document.createElement("input");
            p.RequestPageField.type = "hidden";
            p.RequestPageField.id = p.idPrefix + "-RequestPage";
            p.RequestPageField.name = "RequestPage";
            p.RequestPageField.value = "";
            p.form.appendChild(p.RequestPageField);
            // The grid definition.  It takes 3 components:
            //        a div for the "list"
            p.list = document.createElement("div");
            p.list.id = p.idPrefix + "-list";
            p.container.append(p.list);
            p.table = document.createElement("table");
            p.table.id = p.idPrefix + "-table";
            p.container.append(p.table);
            p.pager = document.createElement("div");
            p.pager.id = p.idPrefix + "-pager";
            p.container.append(p.pager);
            JqGridGlobal.jqGridSettings[p.idPrefix] = p;
        }
        return p;
    }
    function generateGrid(options) {
        var oKey = options.idPrefix;
        // our default options... the expectation is that the end developer sets all of these in the *.html code.
        var p = bindConfiguration(oKey, options);
        // Sanity check
        if ((options.actionButtons) && (options.actionButtons.length > 0)) {
            // Make sure the 'action' type column exists
            var propertyLength = options.properties.length;
            var foundActionColumn = false;
            for (var i = 0; i < propertyLength; i++) {
                if (options.properties[i].type == "action") {
                    foundActionColumn = true;
                    break;
                }
            }
            if (!foundActionColumn) {
                throw "Missing action column which will contains the specified action buttons.";
            }
        }
        // building the grid here.
        var grid = $(p.table).jqGrid({
            url: p.oDataEndPoint,
            editurl: p.oDataEndPoint,
            datatype: "json",
            height: ((p.gridHeight === undefined) ? p.height : p.gridHeight),
            autowidth: true,
            pager: "#" + p.pager.id,
            viewrecords: true,
            caption: p.gridTitle,
            gridview: true,
            // allows the user to sort by multiple columns.  the catch here is that the column ordering in the grid sets the prescedence for the
            // hierarchy of the sort.. so if you have id first, then date, then name, it's going to sort by id, date, and name.  to sort by date,
            // then name, the user will need to drag the "name" field to the first position, then the "date" field to the second position, then
            // click the columns to get the ordering (asc/desc) that they wish.
            multiSort: true,
            multiselect: options.isMultipleSelection,
            sortable: true,
            colNames: getHeaders(p.cols, options),
            colModel: getColumDefinitions(p.cols),
            sortname: options.defaultSortPropertyName,
            sortorder: (options.defaultSortAscending ? "asc" : "desc"),
            rowNum: options.rowCountPerPage,
            rowList: options.rowCountsPerPage,
            rownumbers: options.isRowNumbersVisible,
            scroll: p.useVirtualScrolling,
            onSelectRow: p.onRowSelection,
            onPaging: p.onPageChange,
            onSelectAll: p.onAllSelection,
            onSortCol: p.onSortColumnClick,
            ajaxGridOptions: {
                contentType: "application/json charset=utf-8"
            },
            serializeGridData: function (postData) {
                return setupWebServiceData(postData, p);
            },
            beforeProcessing: function (data, textStatus, jqXHR) {
                //// builds out the total page count for the data returned.
                //var rows = parseInt($(this).jqGrid("getGridParam", "rowNum"), 10);
                //data.total = Math.ceil(data["Count"] / rows); // change to odata.count if using Odata API
                // builds out the total page count for the data returned.
                var rowCount = parseInt($(this).jqGrid("getGridParam", "rowNum"), 10);
                data.page = $("#" + p.idPrefix + "-RequestPage").val();
                data.records = data["odata.count"];
                data.total = Math.ceil(data.records / rowCount); // change to odata.count if using Odata API
                var rows = [];
                for (var i = 0; i < data.value.length; i++) {
                    var item = data.value[i];
                    item.id = item[p.keyPropertyName];
                    rows.push(item);
                }
                data.rows = rows;
            },
            jsonReader: {
                //root: "Items", // the root node of the Json, change to value for OData api.
                repeatitems: false,
            },
            loadError: function (jqXHR, textStatus, errorThrown) {
                var message = "HTTP status code: " + jqXHR.status + "\ntextStatus: " + textStatus + "\nerrorThrown: " + errorThrown;
                alert(message);
            },
            gridComplete: function () {
                setupActionColumn($(p.table), options);
            }
        });
        buildNavigation(grid, options);
        var parentContainer = $("#" + p.containerId);
        $("#sidenav-flyout-btn").on("click", function () {
            setTimeout(function () {
                p.windowResizeDelay = beginResize(grid, parentContainer, p.windowResizeDelay);
            }, 200);
        });
        $(window).resize(function (event) {
            p.windowResizeDelay = beginResize(grid, parentContainer, p.windowResizeDelay);
        });
        return grid;
    }
    DataMatrixJqGridImplementation.generateGrid = generateGrid;
    ;
    function setupActionColumn(grid, options) {
        if (!(options.actionButtons) || (options.actionButtons.length <= 0)) {
            return;
        }
        var actionButtonLength = options.actionButtons.length;
        var ids = grid.jqGrid('getDataIDs');
        for (var i = 0; i < ids.length; i++) {
            var rowId = ids[i];
            var html = "";
            for (var k = 0; k < actionButtonLength; k++) {
                var actionButton = options.actionButtons[k];
                if ((!actionButton.buttonType) || (actionButton.buttonType == 0 /* Button */)) {
                    html += "<button class=\"" + actionButton.buttonClass + "\" onclick=\"DataMatrixJqGridImplementation.handleActionButtonClicked('" + options.elementId + "'," + k + ",'" + rowId + "');\" >";
                    if (actionButton.icon) {
                        html += "<span class=\"" + actionButton.icon + "\"></span>";
                    }
                    html += encodingXml(actionButton.displayName) + "</button>";
                }
                else if (actionButton.buttonType == 1 /* Link */) {
                    html += "<button class=\"btn btn-sm btn-link " + actionButton.buttonClass + "\" onclick=\"DataMatrixJqGridImplementation.handleActionButtonClicked('" + options.elementId + "'," + k + ",'" + rowId + "');\" >";
                    html += encodingXml(actionButton.displayName) + "</button>";
                }
                else {
                    throw "Don't support DataMatrixRowActionButtonType " + actionButton.buttonType;
                }
            }
            grid.jqGrid('setRowData', rowId, { _action: html });
        }
    }
    function handleActionButtonClicked(dataMatrixElementId, actionButtonIndex, rowId) {
        var options = $("#" + dataMatrixElementId).dataMatrix("option");
        var actionButton = options.actionButtons[actionButtonIndex];
        actionButton.onClick(actionButton, rowId);
    }
    DataMatrixJqGridImplementation.handleActionButtonClicked = handleActionButtonClicked;
    function setupWebServiceData(postData, p) {
        // basic posting parameters to the OData service.
        var params = $.extend(false, {
            $top: postData.rows,
            $skip: (parseInt(postData.page, 10) - 1) * postData.rows,
            $inlinecount: "allpages",
        }, p.originalOptions.extraRequestData);
        // if we have an order-by clause to use, then we build it.
        if (postData.sidx) {
            // two columns have the following data:
            // postData.sidx = "{ColumnName} {order}, {ColumnName} "
            // postData.sord = "{order}"
            // we need to split sidx by the ", " and see if there are multiple columns.  If there are, we need to go through
            // each column and get its parts, then parse that for the appropriate columns to build for the sort.
            var splitColumnOrdering = (postData.sidx + postData.sord).split(", ");
            if (splitColumnOrdering.length == 1) {
                params.$orderby = buildColumnSort(splitColumnOrdering[0], p.quoteFilters);
            }
            else {
                var colOrdering = $.map(splitColumnOrdering, function (element, idx) {
                    return buildColumnSort(element, p.quoteFilters);
                });
                params.$orderby = colOrdering.join(", ");
            }
        }
        // if we want to support "in" clauses, we need to follow this stackoverflow article:
        //http://stackoverflow.com/questions/7745231/odata-where-id-in-list-query/7745321#7745321
        // this is for basic searching, with a single term.
        if (postData.searchField) {
            var quoteFilter = findQuoteFilter(postData.searchField, p.quoteFilters);
            params.$filter = ODataExpression(postData.searchOper, postData.searchField, postData.searchString, p.quoteFilters);
        }
        // complex searching, with a groupOp.  This is for if we enable the form for multiple selection criteria.
        if (postData.filters) {
            var filterGroup = $.parseJSON(postData.filters);
            params.$filter = parseFilterGroup(filterGroup, p.quoteFilters);
        }
        // sets the form elements with the filter/group parameters, so that the user can
        // export the data to excel if they so choose.
        $("#" + p.idPrefix + "-Filter").val(params.$filter);
        $("#" + p.idPrefix + "-OrderBy").val(params.$orderby);
        $("#" + p.idPrefix + "-RequestPage").val(postData.page);
        return params;
    }
    function appendExtraRequestData(baseUrl, options) {
        if (options.extraRequestData === undefined) {
            return baseUrl;
        }
        var url = baseUrl;
        if (url.indexOf("?") >= 0) {
            url += "&";
        }
        else {
            url += "?";
        }
        for (var pair in options.extraRequestData) {
            url += pair + "=" + options.extraRequestData[pair] + "&";
        }
        return url;
    }
    function serverErrorTextFormat(data) {
        // We only handle 409 error(Conflict).
        if ((data.status === 409) && (data.responseText) && (data.responseText.length > 0)) {
            var json = JSON.parse(data.responseText);
            return json.value;
        }
        else {
            return "Status: '" + data.statusText + "'. Error code: " + data.status;
        }
    }
    function buildNavigation(grid, options) {
        var key = grid.context.id.split("-")[0];
        var p = bindConfiguration(key, options);
        var addOptions = {
            closeAfterAdd: true,
            mtype: "POST",
            beforeSubmit: function (id) {
                grid.setGridParam({ editurl: appendExtraRequestData(p.oDataEndPoint, options) });
                return [true, ""];
            },
            errorTextFormat: serverErrorTextFormat,
        };
        var editOptions = {
            mtype: "PUT",
            width: 600,
            closeOnEscape: true,
            beforeSubmit: function (dataRow) {
                grid.setGridParam({ editurl: appendExtraRequestData(p.oDataEndPoint + "(" + dataRow[p.table.id + "_id"] + ")", options) });
                return [true, ""];
            },
            errorTextFormat: serverErrorTextFormat,
        };
        var delOptions = {
            mtype: "DELETE",
            beforeSubmit: function (id) {
                grid.setGridParam({ editurl: appendExtraRequestData(p.oDataEndPoint + "(" + id + ")", options) });
                return [true, ""];
            },
            errorTextFormat: serverErrorTextFormat,
        };
        grid.navGrid("#" + p.pager.id, { search: p.originalOptions.isSearchButtonVisible, edit: p.originalOptions.isEditButtonVisible, add: p.originalOptions.isCreateButtonVisible, del: p.originalOptions.isDeleteButtonVisible }, editOptions, addOptions, delOptions, buildSearchOptions(grid, options));
        // adds a little space between buttons
        grid.navSeparatorAdd("#" + p.pager.id, { sepclass: "", sepcontent: " " });
        //// Creates the "Excel" button.
        //grid.jqGrid("navButtonAdd", "#" + p.pager.id, {
        //    caption: "Excel",
        //    buttonicon: "none",
        //    onClickButton: function () { showExportExcelUI(p); },
        //    position: "last",
        //    title: "Export to Excel",
        //    cursor: "pointer"
        //});
        // Creates the "Colunmn" button.
        if (options.isColumnChooserEnabled === true) {
            grid.jqGrid('navButtonAdd', "#" + p.pager.id, {
                caption: "Columns",
                title: "Reorder Columns",
                onClickButton: function () {
                    grid.jqGrid('columnChooser');
                }
            });
        }
        var footerBarButtonLength = options.footerBarButtons.length;
        for (var i = 0; i < footerBarButtonLength; i++) {
            grid.jqGrid('navButtonAdd', "#" + p.pager.id, createFooterButtonObject(grid, options, options.footerBarButtons[i]));
        }
    }
    ;
    function buildSearchOptions(grid, options) {
        return { closeOnEscape: true, multipleSearch: true, closeAfterSearch: true, multipleGroup: true };
    }
    function getSelectedRowIds(grid, options) {
        var selectedRowIds = grid.jqGrid('getGridParam', 'selarrrow');
        if (!options.isMultipleSelection) {
            selectedRowIds = [];
            var selectedRowId = grid.jqGrid('getGridParam', 'selrow');
            if (selectedRowId !== null) {
                selectedRowIds.push(selectedRowId);
            }
        }
        return selectedRowIds;
    }
    DataMatrixJqGridImplementation.getSelectedRowIds = getSelectedRowIds;
    function getRow(grid, rowId) {
        var rowData = grid.jqGrid('getRowData', rowId);
        return rowData;
    }
    DataMatrixJqGridImplementation.getRow = getRow;
    function deleteRow(grid, options, rowId, needConfirmation) {
        var rowIds = [];
        rowIds.push(rowId);
        deleteRows(grid, options, rowIds, needConfirmation);
    }
    DataMatrixJqGridImplementation.deleteRow = deleteRow;
    function deleteRows(grid, options, rowIds, needConfirmation) {
        var key = grid.context.id.split("-")[0];
        var jqGridOptions = JqGridGlobal.jqGridSettings[options.idPrefix];
        var delOptions = {
            mtype: "DELETE",
            beforeSubmit: function (id) {
                grid.setGridParam({ editurl: appendExtraRequestData(options.oDataUrl + "(" + id + ")", options) });
                return [true, ""];
            },
            errorTextFormat: serverErrorTextFormat,
        };
        if (needConfirmation) {
            grid.jqGrid('delGridRow', rowIds, delOptions);
        }
        else {
            throw "Don't support needConfirmation in delGridRow().";
        }
    }
    DataMatrixJqGridImplementation.deleteRows = deleteRows;
    function editRow(grid, options, rowId) {
        var key = grid.context.id.split("-")[0];
        var jqGridOptions = JqGridGlobal.jqGridSettings[options.idPrefix];
        var p = bindConfiguration(key, jqGridOptions);
        var editOptions = {
            closeAfterEdit: true,
            mtype: "PUT",
            width: 600,
            closeOnEscape: true,
            beforeSubmit: function (dataRow) {
                grid.setGridParam({ editurl: appendExtraRequestData(options.oDataUrl + "(" + dataRow[p.table.id + "_id"] + ")", options) });
                return [true, ""];
            },
            errorTextFormat: serverErrorTextFormat,
        };
        grid.jqGrid('setSelection', rowId);
        grid.jqGrid('editGridRow', rowId, editOptions);
    }
    DataMatrixJqGridImplementation.editRow = editRow;
    function createRow(grid, options) {
        //var key = grid.context.id.split("-")[0];
        //var jqGridOptions = JqGridGlobal.jqGridSettings[options.idPrefix];
        //var p = bindConfiguration(key, jqGridOptions);
        var addOptions = {
            closeAfterAdd: true,
            mtype: "POST",
            closeOnEscape: true,
            beforeSubmit: function (id) {
                grid.setGridParam({ editurl: appendExtraRequestData(options.oDataUrl, options) });
                return [true, ""];
            },
            errorTextFormat: serverErrorTextFormat,
        };
        grid.jqGrid('editGridRow', "new", addOptions);
    }
    DataMatrixJqGridImplementation.createRow = createRow;
    function showSearchDialog(grid, options) {
        grid.jqGrid('searchGrid', buildSearchOptions(grid, options));
    }
    DataMatrixJqGridImplementation.showSearchDialog = showSearchDialog;
    function createFooterButtonObject(grid, options, button) {
        return {
            caption: button.displayName,
            title: button.description,
            'class': button.buttonClass,
            onClickButton: function () {
                var selectedRowIds = getSelectedRowIds(grid, options);
                var selectedRows = [];
                var length = selectedRowIds.length;
                if (length > 0) {
                    for (var r = 0; r < length; r++) {
                        var rowData = getRow(grid, selectedRowIds[r]);
                        selectedRows.push(rowData);
                    }
                }
                button.onClick(button, selectedRowIds, selectedRows);
            }
        };
    }
    // sets up resizing the grid in the event that the user shows/hides navigation, or
    // resizes the window.
    function beginResize(grid, container, delay) {
        if (delay !== -1) {
            clearTimeout(delay);
            delay = -1;
        }
        delay = setTimeout(function () {
            var newWidth = container.width();
            grid.setGridWidth(newWidth, true);
            delay = -1;
        }, 100);
        return delay;
    }
    function showExportExcelUI(p) {
        // the dialog to prompt the user for the respective file name.
        $("<div></div>").dialog({
            title: "Save As...",
            height: 150,
            width: 300,
            modal: true,
            buttons: [{
                text: "Ok",
                click: function () {
                    // copy the value from the dialog's text box to the form to be sumbitted's field to hold the file name.
                    var formFileField = $("#" + p.idPrefix + "-filename");
                    var dialogValue = $("#" + p.idPrefix + "-dlgFilename");
                    formFileField.val(dialogValue.val());
                    if (formFileField.val()) {
                        p.form.submit();
                        $(this).dialog("close");
                    }
                    else {
                        alert("File name was not provided!");
                    }
                }
            }, {
                text: "Cancel",
                click: function () {
                    // closes the file.
                    $(this).dialog("close");
                }
            }],
            close: function (event, ui) {
                // desconstructs the Dailog and removes the html elements that are added to the page.
                $(this).dialog("destroy");
                $(this).remove();
            }
        }).html("<label for=\"" + p.idPrefix + "-dlgFilename\">Filename:</label>" + "<input type=\"text\" id=\"" + p.idPrefix + "-dlgFilename\" style=\"float: right; width: 175px;\" />");
    }
    // when dealing with the advanced query dialog, this parses the encapsulating Json object
    // which we will then build the advanced OData expression from.
    function parseFilterGroup(filterGroup, filters) {
        var filterText = "";
        if (filterGroup.groups) {
            if (filterGroup.groups.length) {
                for (var i = 0; i < filterGroup.groups.length; i++) {
                    filterText += "(" + parseFilterGroup(filterGroup.groups[i], filters) + ")";
                    if (i < filterGroup.groups.length - 1) {
                        filterText += " " + filterGroup.groupOp.toLowerCase() + " ";
                    }
                }
                if (filterGroup.rules && filterGroup.rules.length) {
                    filterText += " " + filterGroup.groupOp.toLowerCase() + " ";
                }
            }
        }
        if (filterGroup.rules.length) {
            // fields that are considered as a list should get built as a single
            // odata expression.
            var listFields = $.grep(filterGroup.rules, function (rule, idx) {
                var foundFilter = findQuoteFilter(rule.field, filters);
                if (foundFilter.isList !== undefined) {
                    return foundFilter.isList;
                }
                return false;
            });
            var allListNames = $.map(listFields, function (rule, idx) {
                return rule.field;
            });
            //var distinctFieldNames = $.unique(allListNames);
            var distinctFieldNames = allListNames;
            $.each(distinctFieldNames, function (idx, fieldName) {
                var fieldValues = $.grep(listFields, function (fieldValue, idx) {
                    return fieldValue.field === fieldName;
                });
                var fieldDataValues = $.map(fieldValues, function (rule, idx) {
                    return rule.data;
                });
                var foundFilter = findQuoteFilter(fieldName, filters);
                filterText += ODataListExpression(filterGroup.groupOp.toLowerCase(), fieldName, fieldDataValues, foundFilter);
            });
            var elementFields = $.grep(filterGroup.rules, function (rule, idx) {
                var foundFilter = findQuoteFilter(rule.field, filters);
                if (foundFilter.isList !== undefined) {
                    return !foundFilter.isList;
                }
                return true;
            });
            for (var i = 0; i < elementFields.length; i++) {
                var rule = filterGroup.rules[i];
                var filter = findQuoteFilter(rule.field, filters);
                filterText += ODataExpression(rule.op, rule.field, rule.data, filter);
                if (i < filterGroup.rules.length - 1) {
                    filterText += " " + filterGroup.groupOp.toLowerCase() + " ";
                }
            }
        }
        return filterText;
    }
})(DataMatrixJqGridImplementation || (DataMatrixJqGridImplementation = {}));
var DataMatrixBootgridImplementation;
(function (DataMatrixBootgridImplementation) {
    var bootGridEventNameSpace = ".rs.jquery.bootgrid";
    var BootgridOptions = (function () {
        function BootgridOptions() {
            this.ajax = true;
            this.sorting = true;
            this.multiSort = false;
            this.requestHandler = defaultRequestHandler;
            this.responseHandler = defaultResponseHandler;
            this.labels = {
                all: "All",
                infos: "{{ctx.start}} - {{ctx.end}}　共 {{ctx.total}} 条",
                loading: "正在请求数据...",
                noResults: "未查询到任何结果！",
                refresh: "刷新",
                search: "关键字搜索",
            };
        }
        return BootgridOptions;
    })();
    var BootgridProperty = (function () {
        function BootgridProperty() {
        }
        return BootgridProperty;
    })();
    var BootgridPager = (function () {
        function BootgridPager() {
        }
        BootgridPager.Current = 1;
        BootgridPager.RowCount = 20;
        BootgridPager.Rows = [];
        BootgridPager.Total = 0;
        return BootgridPager;
    })();
    function generateHeader(properties) {
        var headRow = $("<tr>");
        properties.forEach(function (p, i, properties) {
            var $th = $("<th>").text(p.displayName).data("column-id", p.id);
            for (var n in p) {
                if (n !== "displayName" && n !== "id") {
                    $th.data(n, p[n]);
                }
            }
            headRow.append($th);
        });
        return $("<thead>").append(headRow);
    }
    function defaultRequestHandler(request) {
        BootgridPager.Current = request.current;
        BootgridPager.RowCount = request.rowCount;
        return request;
    }
    function defaultResponseHandler(response) {
        BootgridPager.Rows = response["value"];
        BootgridPager.Total = response["odata.count"];
        return { current: BootgridPager.Current, rowCount: BootgridPager.RowCount, rows: BootgridPager.Rows, total: BootgridPager.Total };
    }
    function generateGrid(options) {
        var $container = $("#" + options.elementId), gridId = options.idPrefix + "-grid", $table = $("<table id=\"" + gridId + "\" class=\"table-hover\" style=\"table-layout: auto;\">"), bootGridProperties = convertToBootgridProperties(options), bootGridOptions = convertToBootgridOptions(options, bootGridProperties), $thead = generateHeader(bootGridProperties);
        if (typeof (options.height) !== "undefined") {
            $container.css({ "height": options.height + "px", "overflow-x": "hidden" });
        }
        $container.append($table.append($thead));
        var grid = $table.bootgrid(bootGridOptions).on("loaded" + bootGridEventNameSpace, function () {
            //$("#" + gridId).css("max-height", "400px");
            /* Executes after data is loaded and rendered */
            //grid.find(".command-edit").on("click", function (e) {
            //    alert("You pressed edit on row: " + $(this).data("row-id"));
            //}).end().find(".command-delete").on("click", function (e) {
            //        alert("You pressed delete on row: " + $(this).data("row-id"));
            //    });
        }).on("load" + bootGridEventNameSpace, function () {
            // 
        });
        return grid;
    }
    DataMatrixBootgridImplementation.generateGrid = generateGrid;
    function convertToBootgridOptions(options, bootgridProperties) {
        var opts = new BootgridOptions();
        opts._elementId = options.elementId;
        opts.height = options.height;
        opts.columnSelection = options.isColumnChooserEnabled;
        opts.selection = options.isMultipleSelection;
        opts.rowSelect = options.isMultipleSelection;
        opts.multiSelect = options.isMultipleSelection;
        opts.url = options.oDataUrl;
        opts.post = options.extraRequestData;
        opts.extraQueryString = options.extraRequestData;
        opts.rowCount = options.rowCountsPerPage;
        if (typeof (options.rowCountPerPage) !== "undefined") {
            if ($.inArray(options.rowCountPerPage, opts.rowCount) === -1) {
                opts.rowCount = [options.rowCountPerPage].concat(opts.rowCount);
            }
        }
        // 0 for none, 1 for header, 2 for footer and 3 for both.
        if (options.isHeaderBarHidden === true) {
            if (options.isFooterBarHidden === true) {
                opts.navigation = 3;
            }
            else {
                opts.navigation = 1;
            }
        }
        else if (options.isFooterBarHidden === true) {
            if (options.isHeaderBarHidden === true) {
                opts.navigation = 3;
            }
            else {
                opts.navigation = 2;
            }
        }
        else {
            opts.navigation = 0;
        }
        opts.converters = generateConverters(bootgridProperties);
        opts.formatters = generateFomatters(bootgridProperties);
        return opts;
    }
    function convertToBootgridProperties(options) {
        var properties = [];
        var dataMatrixProperties = options.properties;
        dataMatrixProperties.forEach(function (p, i, dataMatrixProperties) {
            var property = new BootgridProperty();
            property.id = p.name || "";
            property.identifier = p.isKey;
            property.displayName = p.displayName || "";
            property.type = p.type;
            property.typeOptions = p.typeOptions;
            property.columnWidth = p.columnWidth;
            property.searchable = p.isSearchable;
            property.sortable = p.isSortable;
            property.visible = !p.isColumnHidden;
            property.formatter = property.id;
            property.converter = property.id;
            switch (property.type.toLowerCase()) {
                case "action":
                    property.buttons = options.actionButtons;
                    if (property.id.replace(/" "/g, "") === "") {
                        property.id = "_action" + (Math.random() * 10000);
                    }
                case "imagehub":
                case "imageurl":
                case "enum":
                case "enumstring":
                case "enumguid":
                case "boolean":
                    property.searchable = false;
                    property.sortable = false;
                    break;
                case "date":
                case "datetime":
                    property.searchable = false;
                    property.sortable = true;
                    break;
                default:
                    break;
            }
            if ((options.defaultSortPropertyName || "").toLowerCase() === property.id.toLowerCase() || (options.defaultSortPropertyName || "").toLowerCase() === property.displayName.toLowerCase()) {
                property.sortable = true;
                if (typeof (options.defaultSortAscending) != "undefined") {
                    property.order = options.defaultSortAscending ? "asc" : "desc";
                }
            }
            properties.push(property);
        });
        return properties;
    }
    function generateConverters(bootgridProperties) {
        var converters = {};
        bootgridProperties.forEach(function (p, i, bootgridProperties) {
            switch (p.type.toLowerCase()) {
                case "action":
                    break;
                case "imagehub":
                    break;
                case "imageurl":
                    break;
                case "enum":
                    break;
                case "enumstring":
                    break;
                case "enumguid":
                    break;
                case "string":
                    break;
                case "multilinestring":
                    break;
                case "number":
                    break;
                case "boolean":
                    break;
                case "date":
                    converters[p.converter] = {
                        from: function (value) {
                            return value;
                        },
                        to: function (value) {
                            return dateFormat(value, "yyyy-mm-dd");
                        }
                    };
                    break;
                case "datetime":
                    converters[p.converter] = {
                        from: function (value) {
                            return value;
                        },
                        to: function (value) {
                            var dotIndex = value.indexOf('.');
                            if (dotIndex > 0) {
                                value = value.substring(0, dotIndex);
                            }
                            var t = dateFormat(new Date(value.replace("-", "/").replace("T", " ")).addMinutes(-(new Date().getTimezoneOffset())), "yyyy-mm-dd HH:MM:ss");
                            return t;
                        } //{ return dateFormat(value, "yyyy-mm-dd HH:MM"); }//
                    };
                    break;
                default:
                    break;
            }
        });
        return converters;
    }
    function generateFomatters(bootgridProperties) {
        var formatters = {};
        bootgridProperties.forEach(function (p, i, bootgridProperties) {
            switch (p.type.toLowerCase()) {
                case "action":
                    formatters[p.formatter] = actionFormatter;
                    break;
                case "imagehub":
                    formatters[p.formatter] = imagehubFormatter;
                    break;
                case "imageurl":
                    formatters[p.formatter] = imageurlFormatter;
                    break;
                case "enum":
                    formatters[p.formatter] = enumFormatter;
                    break;
                case "enumstring":
                    formatters[p.formatter] = enumStringFormatter;
                    break;
                case "enumguid":
                    formatters[p.formatter] = enumGuidFormatter;
                    break;
                case "string":
                case "multilinestring":
                case "number":
                case "boolean":
                case "date":
                case "datetime":
                default:
                    formatters[p.formatter] = function (column, row) {
                        return row[column.id];
                    };
                    break;
            }
        });
        return formatters;
    }
    function handleActionButtonClicked(dataMatrixElementId, actionButtonIndex, rowId) {
        var options = $("#" + dataMatrixElementId).dataMatrix("option");
        var actionButton = options.actionButtons[actionButtonIndex];
        actionButton.onClick(actionButton, rowId);
    }
    DataMatrixBootgridImplementation.handleActionButtonClicked = handleActionButtonClicked;
    function actionFormatter(column, row) {
        var rowId = (column.identifierColumnId === null) ? row._rowIndex : row[column.identifierColumnId], actionButtons = column.buttons, elementId = column._elementId, html = "";
        for (var i = 0; i < actionButtons.length; i++) {
            var actionButton = actionButtons[i];
            if ((!actionButton.buttonType) || (actionButton.buttonType == 0 /* Button */)) {
                html += "<button class=\"" + actionButton.buttonClass + "\" onclick=\"DataMatrixBootgridImplementation.handleActionButtonClicked('" + elementId + "'," + i + ",'" + rowId + "');\" >";
                if (actionButton.icon) {
                    html += "<span class=\"" + actionButton.icon + "\"></span>";
                }
                html += actionButton.displayName + "</button>";
            }
            else if (actionButton.buttonType == 1 /* Link */) {
                html += "<button class=\"btn btn-sm btn-link " + actionButton.buttonClass + "\" onclick=\"DataMatrixBootgridImplementation.handleActionButtonClicked('" + elementId + "'," + i + ",'" + rowId + "');\" >";
                html += actionButton.displayName + "</button>";
            }
            else {
                throw "Don't support DataMatrixRowActionButtonType " + actionButton.buttonType;
            }
        }
        return html;
    }
    function generateImageUrl(baseUrl, appIdOrName, imageId, scaleType, width, height, extension) {
        var url = baseUrl.replace(/" "/g, "");
        if (url.substr(0, (url.length - 1)) === "/") {
            url = url.substr(0, (url.length - 1));
        }
        return url + "/" + appIdOrName + "/" + imageId.toString() + "_" + scaleType + "_" + width + "_" + height + "." + extension;
    }
    function imagehubFormatter(column, row) {
        var cellvalue = row[column.id], typeOptions = column.typeOptions, html = "";
        var imageExtension = typeOptions.imageExtension;
        if (imageExtension == undefined) {
            imageExtension = "jpg";
        }
        if (typeOptions.isMultiple === false) {
            var url = generateImageUrl(typeOptions.baseUrl, typeOptions.appIdOrName, cellvalue, 3, 100, 100, imageExtension);
            html = "<img src=\"" + url + "\" onerror=\"$(this).remove();\" />";
        }
        else {
            for (var i = 0; i < cellvalue.length; i++) {
                var url = generateImageUrl(typeOptions.baseUrl, typeOptions.appIdOrName, cellvalue[i], 3, 100, 100, imageExtension);
                html += "<img src=\"" + url + "\" onerror=\"$(this).remove();\" />";
            }
        }
        return html;
    }
    function imageurlFormatter(column, row) {
        return "<img src=\"" + row[column.id] + "\" onerror=\"$(this).remove();\" />";
    }
    function enumFormatter(column, row) {
        var cellvalue = row[column.id], typeOptions = column.typeOptions, enumValues = typeOptions["enumValues"], enumHtmlValues = typeOptions["enumHtmlValues"];
        if (enumValues === undefined) {
            throw "enumValues in typeOptions must be defined.";
        }
        if (typeof (enumHtmlValues) === "undefined") {
            return enumValues[cellvalue];
        }
        else {
            return enumHtmlValues[cellvalue];
        }
    }
    function enumStringFormatter(column, row) {
        var cellvalue = row[column.id], typeOptions = column.typeOptions, enumValues = typeOptions["enumValues"], enumHtmlValues = typeOptions["enumHtmlValues"];
        if (enumValues === undefined) {
            throw "enumValues in typeOptions must be defined.";
        }
        if (typeof (enumHtmlValues) === "undefined") {
            return enumValues[cellvalue];
        }
        else {
            return enumHtmlValues[cellvalue];
        }
    }
    function enumGuidFormatter(column, row) {
        var cellvalue = row[column.id], typeOptions = column.typeOptions, enumValues = typeOptions["enumValues"], enumHtmlValues = typeOptions["enumHtmlValues"];
        if (enumValues === undefined) {
            throw "enumValues in typeOptions must be defined.";
        }
        if (typeof (enumHtmlValues) === "undefined") {
            return enumValues[cellvalue];
        }
        else {
            return enumHtmlValues[cellvalue];
        }
    }
    function reload(grid) {
        grid.data(".rs.jquery.bootgrid").reload();
    }
    DataMatrixBootgridImplementation.reload = reload;
    function createRow(grid, options) {
        var $dialog = $("<div>").dialog({
            title: "新建",
            autoOpen: true,
            height: 180,
            width: 400,
            modal: true,
            resizable: false,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                }
            },
            close: function (event, ui) {
                // desconstructs the Dailog and removes the html elements that are added to the page.
                $(this).dialog("destroy");
                $(this).remove();
            }
        }).html("功能建设中...");
    }
    DataMatrixBootgridImplementation.createRow = createRow;
    function getRow(grid, rowId) {
        var rowData = grid.data(".rs.jquery.bootgrid").getRowData(rowId);
        return rowData;
    }
    DataMatrixBootgridImplementation.getRow = getRow;
    function editRow(grid, options, rowId) {
        var $dialog = $("<div>").dialog({
            title: "编辑",
            autoOpen: true,
            height: 180,
            width: 400,
            modal: true,
            resizable: false,
            buttons: {
                "确定": function () {
                    $(this).dialog("close");
                }
            },
            close: function (event, ui) {
                // desconstructs the Dailog and removes the html elements that are added to the page.
                $(this).dialog("destroy");
                $(this).remove();
            }
        }).html("功能建设中...");
    }
    DataMatrixBootgridImplementation.editRow = editRow;
    function deleteRow(grid, options, rowId, needConfirmation) {
        if (needConfirmation === true) {
            var $dialog = $("<div>").dialog({
                title: "删除",
                autoOpen: true,
                height: 180,
                width: 400,
                modal: true,
                resizable: false,
                buttons: {
                    "确定": function () {
                        grid.data(".rs.jquery.bootgrid").deleteRowData(rowId);
                        $(this).dialog("close");
                    },
                    "取消": function () {
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    // desconstructs the Dailog and removes the html elements that are added to the page.
                    $(this).dialog("destroy");
                    $(this).remove();
                }
            }).html("删除所选记录？");
        }
        else {
            grid.data(".rs.jquery.bootgrid").deleteRowData(rowId);
        }
    }
    DataMatrixBootgridImplementation.deleteRow = deleteRow;
    var dateFormat = function () {
        var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g, timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g, timezoneClip = /[^-+\dA-Z]/g, pad = function (val, len) {
            if (len === void 0) { len = 2; }
            val = String(val);
            while (val.length < len)
                val = "0" + val;
            return val;
        };
        // Some common format strings
        var masks = {
            "default": "ddd mmm dd yyyy HH:MM:ss",
            shortDate: "m/d/yy",
            mediumDate: "mmm d, yyyy",
            longDate: "mmmm d, yyyy",
            fullDate: "dddd, mmmm d, yyyy",
            shortTime: "h:MM TT",
            mediumTime: "h:MM:ss TT",
            longTime: "h:MM:ss TT Z",
            isoDate: "yyyy-mm-dd",
            isoTime: "HH:MM:ss",
            isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
            isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
        };
        // Internationalization strings
        var i18n = {
            dayNames: [
                "Sun",
                "Mon",
                "Tue",
                "Wed",
                "Thu",
                "Fri",
                "Sat",
                "Sunday",
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday"
            ],
            monthNames: [
                "Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sep",
                "Oct",
                "Nov",
                "Dec",
                "January",
                "February",
                "March",
                "April",
                "May",
                "June",
                "July",
                "August",
                "September",
                "October",
                "November",
                "December"
            ]
        };
        // Regexes and supporting functions are cached through closure
        return function (date, mask, utc) {
            if (utc === void 0) { utc = false; }
            var dF = dateFormat;
            // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
            if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
                mask = date;
                date = undefined;
            }
            // Passing date through Date applies Date.parse, if necessary
            //date = date.replace(/-/g, '/').replace('T', ' ');
            //var dotIndex = date.indexOf('.');
            //if (dotIndex > 0)//remove milliseconds
            //{
            //    date = date.substring(0, dotIndex);
            //}
            date = date ? new Date(date) : new Date;
            if (isNaN(date))
                throw SyntaxError("invalid date");
            mask = String(masks[mask] || mask || masks["default"]);
            // Allow setting the utc argument via the mask
            if (mask.slice(0, 4) == "UTC:") {
                mask = mask.slice(4);
                utc = true;
            }
            var _ = utc ? "getUTC" : "get", d = date[_ + "Date"](), D = date[_ + "Day"](), m = date[_ + "Month"](), y = date[_ + "FullYear"](), H = date[_ + "Hours"](), M = date[_ + "Minutes"](), s = date[_ + "Seconds"](), L = date[_ + "Milliseconds"](), o = utc ? 0 : date.getTimezoneOffset(), flags = {
                d: d,
                dd: pad(d),
                ddd: i18n.dayNames[D],
                dddd: i18n.dayNames[D + 7],
                m: m + 1,
                mm: pad(m + 1),
                mmm: i18n.monthNames[m],
                mmmm: i18n.monthNames[m + 12],
                yy: String(y).slice(2),
                yyyy: y,
                h: H % 12 || 12,
                hh: pad(H % 12 || 12),
                H: H,
                HH: pad(H),
                M: M,
                MM: pad(M),
                s: s,
                ss: pad(s),
                l: pad(L, 3),
                L: pad(L > 99 ? Math.round(L / 10) : L),
                t: H < 12 ? "a" : "p",
                tt: H < 12 ? "am" : "pm",
                T: H < 12 ? "A" : "P",
                TT: H < 12 ? "AM" : "PM",
                Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
            };
            return mask.replace(token, function ($0) {
                return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
            });
        };
    }();
})(DataMatrixBootgridImplementation || (DataMatrixBootgridImplementation = {}));
//# sourceMappingURL=DataMatrix.js.map