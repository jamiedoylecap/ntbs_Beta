import Vue from "vue";
import { getHeaders } from "../helpers";
import axios from "axios";

type OptionValue = {
    value: string,
    text: string,
    group: string
};

/*
 The component only works for a single select field to filter one or more other fields, 
 any cascading nonsense will require more specific handling.

 'filteringRefs' prop is required to be an array of strings corresponding to a 
 ref containing a select element (if ref is a vue component, requires a 'selectField' ref within that).
 Each string in the array is also an expected response-object key from the endpoint:
 see OnGetGetFilteredListsByTbService in Episode.cshtml.cs

 Additionally to the ref requirements above the input to drive the filtering must be within a ref
 of name 'filterContainer' containing a select element (if ref is a vue component, requires a 'selectField' ref within that).

 waitForChildMount is intended to be used to either trigger filtering on mount of this component,
 or if relevant to be triggered on mount of the filterContainer's vue component.
*/
const FilteredDropdown = Vue.extend({
    props: {
        filterHandlerPath: {
            type: String
        },
        filteringRefs: {
            type: Array
        },
        waitForChildMount: {
            type: Boolean,
            default: true
        }
    },
    mounted: function () {
        if (!this.waitForChildMount) {
            this.filteringMounted();
        }
    },
    methods: {
        filteringMounted: function () {
            this.fetchFilteredLists(this.getFilteringValue());
        },
        filteringChanged: function () {
            this.fetchFilteredLists(this.getFilteringValue());
        },
        getFilteringValue: function () {
            return this.getSelectElementInRef("filterContainer").value;
        },
        fetchFilteredLists: function (value: string) {
            const requestConfig = {
                url: `${window.location.pathname}/${this.filterHandlerPath}`,
                headers: getHeaders(),
                params: {
                    "value": value
                }
            }

            axios.request(requestConfig)
                .then((response: any) => {
                    for (let key in response.data) {
                        if (response.data.hasOwnProperty(key)) {
                            if (this.filteringRefs.indexOf(key) !== -1) {
                                this.updateSelectList(key, response.data[key]);
                            }
                        }
                    }
                });
        },
        updateSelectList: function (refName: string, values: OptionValue[]) {
            const selectElement = this.getSelectElementInRef(refName);
            const currentSelectedValue = selectElement.value;
            const optionInnerHtml = this.generateOptionInnerHtml(values);

            selectElement.innerHTML = optionInnerHtml;
            this.setSelectToValueOrDefault(selectElement, currentSelectedValue);
        },
        setSelectToValueOrDefault: function (selectElement: HTMLSelectElement, targetValue: string) {
            for (let i = 0; i < selectElement.options.length; i++) {
                if (selectElement.options[i].value === targetValue) {
                    selectElement.value = targetValue;
                    return;
                }
            }

            selectElement.value = "";
        },
        getSelectElementInRef(refName: string) {
            const filterValueContainer = this.$refs[refName];
            if (filterValueContainer.$refs) {
                return filterValueContainer.$refs["selectField"];
            }

            return filterValueContainer.getElementsByTagName("select")[0];
        },
        generateOptionInnerHtml(values: OptionValue[]) {
            if (values.some(entry => entry.group)) {
                return this.generateGroupedOptionsInnerHtml(values);
            }

            let optionInnerHtml = "<option value=\"\">Please select</option>";
            values.forEach(item => optionInnerHtml += `<option value="${item.value}">${item.text}</option>`);
            return optionInnerHtml;
        },
        generateGroupedOptionsInnerHtml(values: OptionValue[]) {
            let optionInnerHtml = "<option value=\"\">Please select</option>";

            values.sort((a, b) => {
                if (a.group > b.group) { return -1 }
                if (a.group < b.group) { return 1 }
                return 0;
            });

            let currentGroup = values[0].group;
            optionInnerHtml += `<optgroup label="${currentGroup}">`;

            values.forEach(item => {
                if (item.group !== currentGroup) {
                    optionInnerHtml += "</optgroup>";
                    currentGroup = item.group;
                    optionInnerHtml += `<optgroup label="${currentGroup}">`;
                }

                optionInnerHtml += `<option value="${item.value}">${item.text}</option>`;
            });

            optionInnerHtml += "</optgroup>";
            return optionInnerHtml;
        }
    }
});

export default FilteredDropdown;
