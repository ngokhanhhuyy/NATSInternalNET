function __vite__mapDeps(indexes) {
  if (!__vite__mapDeps.viteFileDeps) {
    __vite__mapDeps.viteFileDeps = ["assets/ValidationMessage-Bb04LYJD.js","assets/index-t81Ut7qy.js","assets/index-DTuIwUSz.css"]
  }
  return indexes.map((i) => __vite__mapDeps.viteFileDeps[i])
}
import{d as f,ah as v,aj as h,ai as g,a as n,c as i,b as e,w as d,v as p,n as c,u as t,A as m,R as y,F as u,f as V,t as N,H as D,J as j}from"./index-t81Ut7qy.js";const w=e("div",{class:"block-header bg-primary-subtle border border-primary-subtle px-3 py-2"},[e("span",{class:"text-primary small fw-bold"},"THÔNG TIN NHÂN VIÊN")],-1),x={class:"block-body block-body-user-information border border-top-0 border-default rounded-bottom-3 gx-3 p-2 pb-3 row"},k={class:"col col-sm-6 col-12 mb-3"},C={class:"form-group"},M=e("label",{class:"form-label small fw-bold"},"Ngày gia nhập",-1),U={class:"col col-sm-6 col-12 mb-3"},I={class:"form-group"},S=e("label",{class:"form-label small fw-bold"},"Vị trí",-1),T=["value"],A={class:"col col-12"},B={class:"form-group"},E={class:"form-group"},G=e("label",{class:"form-label small fw-bold"},"Ghi chú",-1),O=f({__name:"UserUserInfoUpsert",props:v({roleOptions:{}},{modelValue:{required:!0},modelModifiers:{}}),emits:["update:modelValue"],setup(_){const a=D(()=>j(()=>import("./ValidationMessage-Bb04LYJD.js"),__vite__mapDeps([0,1,2]))),r=h("modelState"),s=g(_,"modelValue");return(b,l)=>(n(),i(u,null,[w,e("div",x,[e("div",k,[e("div",C,[M,d(e("input",{type:"date",class:c(["form-control",t(r).inputClass("joiningDate")]),"onUpdate:modelValue":l[0]||(l[0]=o=>s.value.joiningDate=o)},null,2),[[p,s.value.joiningDate]]),m(t(a),{"property-path":"joiningDate"})])]),e("div",U,[e("div",I,[S,d(e("select",{class:c(["form-select",t(r).inputClass("role")]),"onUpdate:modelValue":l[1]||(l[1]=o=>s.value.role=o)},[(n(!0),i(u,null,V(b.roleOptions.items,o=>(n(),i("option",{value:o,key:o.name},N(o.displayName),9,T))),128))],2),[[y,s.value.role]]),m(t(a),{"property-path":"role"})])]),e("div",A,[e("div",B,[e("div",E,[G,d(e("textarea",{class:c(["form-control",t(r).inputClass("note")]),placeholder:"Ghi chú ..",maxlength:"255","onUpdate:modelValue":l[2]||(l[2]=o=>s.value.note=o)},`\r
                    `,2),[[p,s.value.note]]),m(t(a),{"property-path":"joiningDate"})])])])])],64))}});export{O as default};
