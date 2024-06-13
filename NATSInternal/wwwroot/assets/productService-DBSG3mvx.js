var u=Object.defineProperty;var d=(e,t,a)=>t in e?u(e,t,{enumerable:!0,configurable:!0,writable:!0,value:a}):e[t]=a;var i=(e,t,a)=>(d(e,typeof t!="symbol"?t+"":t,a),a);import{a as r}from"./brandModels-D-3gM8Vf.js";import{a as l}from"./productCategoryModels-D-oOaeWZ.js";import{a6 as h,a8 as c,a5 as s}from"./index-t81Ut7qy.js";class m{constructor(t){i(this,"id");i(this,"name");i(this,"unit");i(this,"price");i(this,"thumbnailUrl");const a=c();this.id=t.id,this.name=t.name,this.unit=t.unit,this.price=t.price,this.thumbnailUrl=t.thumbnailUrl?a.getPhotoUrl(t.thumbnailUrl):a.getDefaultPhotoUrl()}}class U{constructor(t,a){i(this,"categoryName",null);i(this,"brandId",null);i(this,"page",1);i(this,"items",[]);i(this,"pageCount",0);a&&(this.brandId=a)}mapFromResponseDto(t){var a;this.items=((a=t.items)==null?void 0:a.map(n=>new m(n)))||[],this.pageCount=t.pageCount}toRequestDto(){return{categoryName:this.categoryName,brandId:this.brandId,page:this.page}}}class F{constructor(t){i(this,"id");i(this,"name");i(this,"description");i(this,"unit");i(this,"price");i(this,"vatFactor");i(this,"isForRetail");i(this,"isDiscontinued");i(this,"createdDateTime");i(this,"updatedDateTime");i(this,"thumbnailUrl");i(this,"category");i(this,"brand");const a=h(),n=c();this.id=t.id,this.name=t.name,this.description=t.description,this.unit=t.unit,this.price=t.price,this.vatFactor=t.vatFactor,this.isForRetail=t.isForRetail,this.isDiscontinued=t.isDiscontinued,this.createdDateTime=a.toDisplayDateTime(t.createdDateTime),this.updatedDateTime=a.toDisplayDateTime(t.updatedDateTime),this.thumbnailUrl=t.thumbnailUrl?n.getPhotoUrl(t.thumbnailUrl):n.getPhotoUrl("/images/default.jpg"),this.category=t.category&&new l(t.category),this.brand=t.brand&&new r(t.brand)}}class p{constructor(t){i(this,"id",0);i(this,"name","");i(this,"description","");i(this,"unit","");i(this,"price",0);i(this,"vatFactor",0);i(this,"isForRetail",!0);i(this,"isDiscontinued",!1);i(this,"thumbnailUrl",null);i(this,"thumbnailFile",null);i(this,"thumbnailChanged",!1);i(this,"category",null);i(this,"brand",null);if(t){const a=c();this.id=t.id,this.name=t.name,this.description=t.description||"",this.unit=t.unit,this.price=t.price,this.vatFactor=t.vatFactor,this.isForRetail=t.isForRetail,this.isDiscontinued=t.isDiscontinued,this.thumbnailUrl=t.thumbnailUrl?a.getPhotoUrl(t.thumbnailUrl):a.getPhotoUrl("/images/default.jpg"),this.category=t.category&&new l(t.category),this.brand=t.brand&&new r(t.brand)}}toRequestDto(){return{name:this.name,description:this.description||null,unit:this.unit,price:this.price,vatFactor:this.vatFactor,isForRetail:this.isForRetail,isDiscontinued:this.isDiscontinued,thumbnailFile:this.thumbnailFile,thumbnailChanged:this.thumbnailChanged,category:this.category&&this.category.toRequestDto(),brand:this.brand&&this.brand.toRequestDto()}}}function P(){const e=s();return{async getListAsync(t){return await e.getAsync("/product/list",t)},async getDetailAsync(t){return await e.getAsync(`/product/${t}/detail`)},async createAsync(t){return await e.postAsync("/product/create",t)},async updateAsync(t,a){return await e.putAndIgnoreAsync(`/product/${t}/update`,a)},async deleteAsync(t){return await e.deleteAndIgnoreAsync(`/product/${t}/delete`)}}}export{U as P,F as a,p as b,P as u};
