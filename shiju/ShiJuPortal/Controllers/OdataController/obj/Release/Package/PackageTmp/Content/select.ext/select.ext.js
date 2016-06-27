// sample dataSource:
//{
//    metadata: {
//        type: "User",
//        title: "Users",
//        description: "The list of users",
//        iconUrl: "",
//        id: "Id",
//        text: ["Name"],//, "Email"],
//        resultMaxHeight: 100,
//        tagCSS: {
//            "color": "#fff",
//            "background-color": "#007acc"
//        }
//    },
//    tags: [
//        { Id: "U-001", Name: "Junming Huang", Email: "junminghuang@yipinapp.com" }
//    ],
//    data: [
//        { Id: "U-001", Name: "Junming Huang", Email: "junminghuang@yipinapp.com" },
//        { Id: "U-002", Name: "Test Huang", Email: "testhuang@yipinapp.com" }
//    ],
//    ajax: {
//        //url: "@Url.Action("GetUsers")",
//        dataType: "json",
//        data: {},
//        buildQueryStrings: function(keyword) {
//            return { "keyword": keyword, timestamp: new Date().getTime() };
//        },
//        beforeSend: function (jqXHR) {

//        },
//        success: function () {

//        }
//    },
//    render: function (obj, metadata) {
//        alert("Type: " + metadata.type + "\r\nName: " + obj.Name);
//    },
//    complete: function () {

//    }
//},
//{
//    metadata: {
//        type: "Organization",
//        title: "Organization list",
//        description: "The list of organizations",
//        iconUrl: "",
//        id: "Id",
//        text: ["Name"],//, "Email"],
//        resultMaxHeight: 100,
//    },
//    data: [
//        { Id: "G-001", Name: "Server", Description: "Server group" },
//        { Id: "G-002", Name: "Test", Description: "Test group" }
//    ],
//    ajax: {
//        //url: "@Url.Action("GetOrganizations")",
//        dataType: "json",
//        data: {},
//        buildQueryStrings: function (keyword) {
//            return { "keyword": keyword, timestamp: new Date().getTime() };
//        },
//        beforeSend: function (jqXHR) {

//        },
//        success: function () {

//        }
//    },
//    render: function (obj, metadata) {
//        alert("Type: " + metadata.type + "\r\nName: " + obj.Name);
//    },
//    complete: function () {

//    }
//}

(function ($) {
    $.widget("yp.selectExt", {
        options: {
            placeholder: "输入关键字搜索...",
            dataSource: [

            ],
            muitiple: true,
            duplicate: false,
            minInputLength: 1//,
            //maxSelectionSize: 2,
            //dropDownMaxHeight: 600    // if not configed: height=auto;
        },
        values: function () {
            return this._values;
        },
        clear: function () {
            for (var i = 0; i < this._values.length; i++) {
                this._values[i].data = [];
            }
            $(".select-ext-tags > .select-ext-tag").remove();
        },
        _values: [],
        _results: [],
        _inputInitWidth: 0,
        _inputOrginHeight: 0,
        _inputMarginPaddingWidth: 0,
        _inputMinWidth: 40,//px
        _containerId: '',
        _create: function () {
            var options = this.options,
                ele = this.element,
                eleParent = ele.parent(),
                _containerId = "select-ext-container-" + (Math.random() * 10000),
                container = $("<div id=\"" + _containerId + "\" class=\"select-ext-container\"></div>");

            ele.addClass("keywords").css({ "border": "none", "box-shadow": "none", "padding": "0 0 2px 2px" });
            this._inputOrginHeight = ele.height()
                                    + parseInt(ele.css("margin-top"))
                                    + parseInt(ele.css("margin-bottom"))
                                    + parseInt(ele.css("padding-top"))
                                    + parseInt(ele.css("padding-bottom"));
            this._inputMarginPaddingWidth = parseInt(ele.css("margin-left"))
                                            + parseInt(ele.css("margin-right"))
                                            + parseInt(ele.css("padding-left"))
                                            + parseInt(ele.css("padding-right"));
            ele.prop("placeholder", options.placeholder)
               .prop("autocomplete", "off")
               .prop("autocorrect", "off")
               .prop("autocapitalize", "off")
               .prop("spellcheck", "false")
               .on("focus", function () {
                   $(this).parents(".select-ext-container").addClass("select-ext-container-focus");
               }).on("blur", { context: this }, function (event) {
                   var plgin = event.data.context;
                   plgin._getContainer().removeClass("select-ext-container-focus");
               }).on("keyup", { context: this }, this._inputKeyup);

            var containerWidth = calculateElementWidth(ele);
            container.css({ "width": containerWidth, "height": "auto", "min-height": this._inputOrginHeight });
            container.on("click", function () {
                $(this).find(".select-ext-input :input").first().focus();
                stopBubble(event);
            });

            // when container onblur, hide the results container
            window.document.onclick = function (e) {
                if (typeof containerId != 'undefined') {
                    if (e.target.id !== containerId) {
                        container.find(".select-ext-results-container").hide();
                    }
                }
            };

            this._inputInitWidth = container.width() - this._inputMarginPaddingWidth;

            var tags = $("<ul class=\"select-ext-tags\">");
            tags.append($("<li class=\"select-ext-input\">").append(ele.css({ "width": "auto" })));//.width(this._inputInitWidth)));

            eleParent.append(container.append(tags));
        },
        // initilize: after _create
        _init: function () {
            var options = this.options;
            if (options.dataSource) {
                if (isArray(options.dataSource)) { //isArray
                    // Generate a unique type
                    for (var i = 0; i < options.dataSource.length; i++) {
                        var metadata = options.dataSource[i].metadata;
                        if (typeof (metadata.type) === "undefined") {
                            metadata.type = "DataSource_" + i;
                        }
                        // Initlize values schema
                        this._values.push({ type: metadata.type, data: [] });
                    }
                    // render static data
                    for (var i = 0; i < options.dataSource.length; i++) {
                        if (typeof options.dataSource[i].tags !== "undefined") {
                            for (var j = 0; j < options.dataSource[i].tags.length; j++) {
                                this._addTag(options.dataSource[i].metadata, options.dataSource[i].tags[j]);
                            }
                        }
                    }

                } else {    // isObject
                    var metadata = options.dataSource.metadata;
                    if (typeof (metadata.type) === "undefined") {
                        metadata.type = "DataSource_0";
                    }

                    // Initlize values schema
                    this._values.push({ type: metadata.type, data: [] });

                    if (typeof options.dataSource.tags !== "undefined") {
                        for (var j = 0; j < options.dataSource.tags.length; j++) {
                            this._addTag(metadata, options.dataSource.tags[j])
                        }
                    }
                }
            }

            this._refresh();
        },
        _addTag: function (metadata, newObj) {
            var options = this.options;
            var isAllow = true;

            var currentValues = [];
            for (var i = 0; i < this._values.length; i++) {
                v = this._values[i];
                for (var j = 0; j < v.data.length; j++) {
                    currentValues.push(v.data[j]);
                }
            }

            if (options.muitiple === false && currentValues.length > 0) {
                isAllow = false;
            } else if (typeof (options.maxSelectionSize) !== "undefined" && currentValues.length >= parseInt(options.maxSelectionSize)) {
                isAllow = false;
            } else if (options.duplicate === false) {
                var v;
                for (var i = 0; i < this._values.length; i++) {
                    v = this._values[i];
                    if (v.type === metadata.type) {
                        for (var j = 0; j < v.data.length; j++) {
                            if (v.data[j][metadata.id] === newObj[metadata.id]) {
                                isAllow = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (isAllow === true) {
                var v;
                for (var i = 0; i < this._values.length; i++) {
                    v = this._values[i];
                    if (this._values[i].type === metadata.type) {
                        v.data.push(newObj);
                        break;
                    }
                }
                this._renderTag(metadata, newObj);
            }
        },
        _renderTag: function (metadata, obj) {
            var tags = this._getContainer().children("ul.select-ext-tags").first(),
                id = obj[metadata.id],
                text = this._buildText(metadata, obj);

            metadata.tagCSS = metadata.tagCSS || {};

            var tag = $("<li class=\"select-ext-tag\"></li>").data("id", id);
            tag.css({ "max-width": this._inputInitWidth });

            var textDiv = $("<div>").css(metadata.tagCSS);
            if (metadata.iconUrl) {
                var icon = $("<img src=\"" + metadata.iconUrl + "\" />");
                textDiv.append(icon);
            }
            var textSpan = $("<span>").text(text);
            textDiv.append(textSpan);

            var a = $("<a href=\"javascript:void(0);\" class=\"\">&nbsp;</a>");
            a.on("click", { context: this, metadata: metadata, id: id, tag: tag }, function (event) {
                var params = event.data;
                params.context._removeTag(params.metadata, params.id, params.tag);
            });

            textDiv.css({ "max-width": (this._inputInitWidth - parseInt(calculateElementWidth(tag))) });
            tag.append(textDiv).append(a);
            tag.insertBefore(tags.find("li.select-ext-input"));

            this._refresh();
        },
        _removeTag: function (metadata, id, tag) {
            var v;
            for (var i = 0; i < this._values.length; i++) {
                v = this._values[i];
                if (v.type === metadata.type) {
                    for (var j = 0; j < v.data.length; j++) {
                        if (v.data[j][metadata.id] == id) {
                            v.data.splice(j, 1);
                            break;
                        }
                    }
                }
            }

            if (typeof (tag) !== "undefined") {
                $(tag).remove();
            }

            this._refresh();
        },
        _getContainer: function () {
            return this.element.parents(".select-ext-container");
        },
        _getResultsContainer: function () {
            return this._getContainer().find(".select-ext-results-container").first();
        },
        _refresh: function () {
            var ele = this.element,
                options = this.options,
                container = this._getContainer(),
                resultsContainer = container.find(".select-ext-results-container").first();
            if (resultsContainer.length <= 0) {
                resultsContainer = $("<div class=\"select-ext-results-container\">").css({ "width": container.width() });
                if (options.dropDownMaxHeight) {
                    resultsContainer.css({ "max-height": parseInt(options.dropDownMaxHeight) + "px" });
                }
                if (options.dataSource) {
                    if (isArray(options.dataSource)) {
                        for (var i = 0; i < options.dataSource.length; i++) {
                            this._buildDataSourceResults(options.dataSource[i].metadata, resultsContainer);
                        }
                    } else {
                        this._buildDataSourceResults(options.dataSource.metadata, resultsContainer);
                    }
                }
                container.append(resultsContainer);
            }

            // adjust input width
            var containerWidth = calculateElementWidth(container);
            var tags = $("#txtTest").parents(".select-ext-tags").first();
            var tagsMarginPaddingWidth = (containerWidth - container.width()) + (calculateElementWidth(tags) - tags.width());
            var totalTags = tags.find("li.select-ext-tag");
            var totalTagsWidth = tagsMarginPaddingWidth;
            for (var i = 0; i < totalTags.length; i++) {
                var currentTagWidth = calculateElementWidth(totalTags[i]);
                totalTagsWidth += currentTagWidth;
                if (totalTagsWidth >= containerWidth) {
                    totalTagsWidth = tagsMarginPaddingWidth + currentTagWidth;
                }
            }
            var inputWidth = containerWidth - (totalTagsWidth);
            ele.css("width", ((inputWidth >= this._inputMinWidth) ? inputWidth : (containerWidth - this._inputMarginPaddingWidth)) - 4);// 4 is input's padding
            ele.height($(totalTags[0]).height()); // Hard code, same height with tag
            ele.val('');

            resultsContainer.css("top", parseInt(container.height()) + "px").hide();
        },
        _getDataSource: function (type) {
            if (typeof type === "undefined") {
                return this.options.dataSource;
            } else {
                for (var i = 0; i < this.options.dataSource.length; i++) {
                    var dataSource = this.options.dataSource[i];
                    if (metadata.type === type) {
                        return dataSource;
                    }
                }
                return null;
            }
        },
        _buildDataSourceResults: function (metadata, resultsContainer) {
            var title = $("<div class=\"select-ext-results-title\" data-type=\"" + metadata.type + "\">");
            if (metadata.iconUrl) {
                var icon = $("<img src=\"" + metadata.iconUrl + "\" />");
                title.append(icon);
            }

            var ul_results = $("<ul class=\"select-ext-results\" data-type=\"" + metadata.type + "\">");
            if (metadata.resultMaxHeight) {
                ul_results.css({ "max-height": parseInt(metadata.resultMaxHeight) + "px" });
            }

            var span = $("<span></span>").text(metadata.title);
            var a = $("<a href=\"javascript:void(0);\" class=\"arrow arrow-up\" title=\"收起\"></a>");

            title.on("click", { ul: ul_results }, function (event) {
                var arrow = $(this).find("a"),
                    ul = $(event.data.ul);
                if (arrow.hasClass("arrow-up")) {
                    arrow.removeClass("arrow-up").addClass("arrow-down").prop("title", "展开");
                    ul.slideUp("fast");
                } else if (arrow.hasClass("arrow-down")) {
                    arrow.removeClass("arrow-down").addClass("arrow-up").prop("title", "收起");
                    ul.slideDown("fast");
                }
            });

            title.append(span).append(a);
            resultsContainer.append(title).append(ul_results);
        },
        _buildText: function (metadata, obj) {
            var text = "";
            if (typeof metadata.text !== "object") {
                text = obj[metadata.text];
            } else if (isArray(metadata.text)) {
                if (metadata.text.length >= 0) {
                    text = obj[metadata.text[0]];
                }
                for (var h = 1; h < metadata.text.length; h++) {
                    text += "(" + obj[metadata.text[h]] + ")";
                }
            }

            return text;
        },
        _inputKeyup: function (event) {
            var self = $(this),
                keyword = self.val(),
                params = event.data,
                plgin = params.context,
                options = plgin.options,
                resultsContainer = plgin._getResultsContainer();

            // char only in [A-Z,a-z,0-9](65~105)
            if ((event.keyCode < 48 || event.keyCode > 105)
                    && event.keyCode !== 8  // 8 = BackSpace
                    && event.keyCode !== 46 // 46 = Delete
                    && event.keyCode !== 32 // 32 = Space
                    && event.keyCode !== 192    //192 = `
                    && event.keyCode !== 189    //189 = -
                    && event.keyCode !== 187    //187 = =
                    && event.keyCode !== 220    //220 = \
                    && event.keyCode !== 219    //219 = [
                    && event.keyCode !== 221    //221 = ]
                    && event.keyCode !== 186    //186 = ;
                    && event.keyCode !== 222    //222 = '
                    && event.keyCode !== 188    //188 = ,
                    && event.keyCode !== 190    //190 = .
                    && event.keyCode !== 191    //191 = /
                    && event.keyCode !== 110    //110 = .
                    && event.keyCode !== 111    //111 = /
                    && event.keyCode !== 106    //106 = *
                    && event.keyCode !== 109    //109 = -
                    && event.keyCode !== 107    //107 = +
                    && event.keyCode !== 229    //229 = 中文输入法会得到这个Code
                ) {
                return;
            }

            if (keyword === "" || keyword.length < options.minInputLength) {
                resultsContainer.hide();
            } else {
                if (options.dataSource) {
                    if (isArray(options.dataSource)) {
                        for (var i = 0; i < options.dataSource.length; i++) {
                            plgin._renderDataSource(options.dataSource[i]);
                        }
                    } else {
                        plgin._renderDataSource(options.dataSource);
                    }
                }

                // By defalut: expand results
                var result_titles = resultsContainer.find("div.select-ext-results-title");
                var ul_results = resultsContainer.find("ul.select-ext-results");
                for (var i = 0; i < result_titles.length; i++) {
                    var title = $(result_titles[i]),
                        arrow = title.find("a.arrow").first();
                    if (arrow.hasClass("arrow-down")) {
                        arrow.removeClass("arrow-down").addClass("arrow-up").prop("title", "收起");
                        $(ul_results[i]).slideDown("fast");
                    }
                }

                resultsContainer.show();
            }
        },
        _renderDataSource: function (dataSource) {
            var plgin = this,
                ele = this.element,
                calling = false,
                metadata = dataSource.metadata,
                ajaxSettings = dataSource.ajax,
                resultsContainer = this._getResultsContainer(),
                // clean obsolete results
                title = resultsContainer.find(".select-ext-results-title[data-type=" + metadata.type + "]").first(),
                ul_results = resultsContainer.find(".select-ext-results[data-type=" + metadata.type + "]").first().empty();
            plgin._results = [];

            if (typeof (ajaxSettings) !== "undefined") {
                var url = ajaxSettings.url,
                    queryString = (url.indexOf('?') > 0) ? "&" : "?";
                if (typeof (ajaxSettings.buildQueryStrings) !== "undefined") {
                    var urlParameters = ajaxSettings.buildQueryStrings(ele.val());
                    for (var key in urlParameters) {
                        queryString += (key + "=" + urlParameters[key] + "&");
                    }
                }
                url += queryString;

                $.ajax(
                    $.extend({
                        beforeSend: function (jqXHR) {  // do something before webservice call
                            ul_results.empty().append($("<li class=\"tips\">正在搜索...</li>"));
                        },
                        success: function (result, textStatus, jqXHR) {
                            ul_results.empty();

                            var totalData = result;
                            if (typeof dataSource.data !== "undefined") {
                                for (var i = 0; i < dataSource.data.length; i++) {
                                    var obj = dataSource.data[i];
                                    var text = plgin._buildText(metadata, obj);
                                    if (text.toUpperCase().indexOf(ele.val().toUpperCase()) >= 0) {
                                        totalData.push(obj);
                                    }
                                }
                                totalData.concat(result);
                            }
                            if (totalData.length <= 0) {
                                ul_results.append($("<li class=\"tips\">未搜索到任何结果</li>"));
                            } else {
                                for (var j = 0; j < totalData.length; j++) {
                                    plgin._renderRow(ul_results, dataSource, totalData[j]);
                                }
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            if (jqXHR.readyState != 0) {
                                ul_results.empty().append($("<li class=\"tips\" style=\"color:red;\">数据请求异常</li>"));
                            }
                        },
                        complete: function (jqXHR, textStatus) {
                            // ajax completed
                        }
                    }, ajaxSettings, { url: url }));
            } else {
                var totalData = [];
                if (typeof dataSource.data !== "undefined") {
                    for (var i = 0; i < dataSource.data.length; i++) {
                        var obj = dataSource.data[i];
                        var text = plgin._buildText(metadata, obj);
                        if (text.toUpperCase().indexOf(ele.val().toUpperCase()) >= 0) {
                            totalData.push(obj);
                        }
                    }
                }
                if (totalData.length <= 0) {
                    ul_results.append($("<li class=\"tips\">未搜索到任何结果</li>"));
                } else {
                    for (var j = 0; j < totalData.length; j++) {
                        plgin._renderRow(ul_results, dataSource, totalData[j]);
                    }
                }
            }

            if (dataSource.complete) {  // dataSource render completed
                dataSource.complete();
            }
        },
        _renderRow: function (ul_results, dataSource, obj) {
            var metadata = dataSource.metadata,
                result = { type: metadata.type, obj: obj };
            for (var i = 0; i < this._results.length; i++) {
                if (this._results[i].type === result.type && this._results[i].obj[metadata.id] === result.obj[metadata.id]) {
                    return;
                }
            }

            var li = $("<li class=\"item\">").data("type", metadata.type);
            if (typeof dataSource.render === "function") {
                li.append(dataSource.render(obj, metadata));
            } else {
                var content = $("<div class=\"content\">"),
                    //img = $("<div>").append($("<img />")),
                    text = $("<span>").text(obj[metadata.text]);

                //content.append(img);
                content.append(text);
                li.append(content);
            }

            li.on("click", { context: this, metadata: metadata, obj: obj }, function (event) {
                var params = event.data,
                    plgin = params.context;
                plgin._addTag(params.metadata, params.obj);
            })

            ul_results.append(li);
            this._results.push(result);
        }
    });

    function calculateElementWidth(element) {
        var fullWidth = 0,
            ele = $(element);

        fullWidth = ele.width();
        if (isNaN(ele.css("border-left-width")) !== false) {
            fullWidth += parseInt(ele.css("border-left-width"));
        }
        if (isNaN(ele.css("border-right-width")) !== false) {
            fullWidth += parseInt(ele.css("border-right-width"));
        }
        if (isNaN(ele.css("margin-left")) !== false) {
            fullWidth += parseInt(ele.css("margin-left"));
        }
        if (isNaN(ele.css("margin-right")) !== false) {
            fullWidth += parseInt(ele.css("margin-right"));
        }
        if (isNaN(ele.css("padding-left")) !== false) {
            fullWidth += parseInt(ele.css("padding-left"));
        }
        if (isNaN(ele.css("padding-right")) !== false) {
            fullWidth += parseInt(ele.css("padding-right"));
        }
        //if (!!window.ActiveXObject || "ActiveXObject" in window) {    //IE 6-11
        //    fullWidth = fullWidth + 2;
        //} else {
        //    fullWidth = fullWidth + 1;
        //}

        return fullWidth;
    }

    function stopBubble(e) {
        if (e && e.stopPropagation) {
            e.stopPropagation();
        } else {
            window.event.cancelBubble = true;
        }
    }

    function isArray(obj) {
        if (typeof (obj) === "object") {
            if (typeof (obj.constructor) === "function" && obj.constructor === Array) { //isArray
                return true;
            }
        }
        return false;
    }

})(jQuery);