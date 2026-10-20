import React,{useState}from 'react';
export function Tooltip({text,children}){
const [show,setShow]=useState(false);
return React.createElement('span',{style:{position:'relative',display:'inline-flex'},onMouseEnter:()=>setShow(true),onMouseLeave:()=>setShow(false)},
children,
show&&React.createElement('span',{style:{position:'absolute',bottom:'calc(100% + 6px)',left:'50%',transform:'translateX(-50%)',
background:'var(--neutral-grey-190)',color:'var(--neutral-white)',fontFamily:'var(--font-sans-text)',fontSize:'var(--fs-caption)',
padding:'4px 8px',borderRadius:'var(--radius-sm)',whiteSpace:'nowrap',boxShadow:'var(--shadow-4)'}},text));
}
