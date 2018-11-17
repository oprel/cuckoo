module ring(outerRadius,innerRadius,thickness,outerSegmentAngle=30,innerSegmentAngle=30)
// creates a hollow cylinder
// outerRadius and innerRadius should be self-explanatory
// thickess is along z
// outerSegmentAngle is an optional override of the system variable $fa
// innerSegmentAngle is an optional override of the system variable $fa
{
  difference()
    {
      cylinder(thickness,outerRadius,outerRadius,$fn=outerSegmentAngle);
      cylinder(thickness,innerRadius,innerRadius,,$fn=innerSegmentAngle);
    }
}

module tooth(toothLength,thickness,toothLean,toothSharpness)
// creates a triangular tooth, sharp end at origin
// toothLength is measured along longest side
// thickess is along z
// toothLean measures how far the tooth leans over, in degrees
// toothSharpness measures how sharp the tooth is, in degrees
{
      rotate(-toothLean,[0,0,1])
      rotate(180,[0,0,1])
      difference()
      {
        cube([toothLength,toothLength,thickness]);
        rotate(toothSharpness,[0,0,1])
        cube([toothLength*2,toothLength,thickness]);
      }
}

module ringTooth(outerRadius,innerRadius,thickness,numberTeeth,toothLength)
// creates a ring with the child spaced regularly along the outside edge of the inner radius
// outerRadius and innerRadius should be self-explanatory
// thickess is along z 
// numberTeeth controls how many instances of the child are used
// toothlength is used to offset the teeth outward, if needed
{
  union()
  {
    ring(outerRadius,innerRadius,thickness);
    for ( i=[0:numberTeeth])
   {
      rotate(i*360/numberTeeth,[0,0,1])
      translate([innerRadius+toothLength,0,0]) 
      child(0);
    }
  }
}

module escapementWheel(radius,rimWidth,thickness,numberTeeth,toothLength,toothLean,toothSharpness,numberSpokes,spokeWidth,hubWidth,bore)
// creates an escapement wheel
// radius is the outer radius measured from the tooth tips
// rimWidth is the width of the rim of the wheel, measured radially
// thickess is along z
// numberTeeth controls how many teeth the wheel has
// toothLength is the length of each tooth, 
// toothLean measures how far the tooth leans over clockwise, in degrees
// toothSharpness measures how sharp the tooth is, in degrees
// numberSpokes controls the number of spokes of the wheel
// spokeWidth controls the width of the spokes of the wheel
// hubWidth controls the width of the hub
// bore controls the size of the bore in the hub
{
  rotate(4)union()
  {
    ringTooth(radius-toothLength+rimWidth,radius-toothLength,thickness,numberTeeth,toothLength) tooth(toothLength,thickness,toothLean,toothSharpness);
    for ( j=[0:numberSpokes-1])
   {
      rotate(360/numberSpokes*j,[0,0,1]) 
      translate([bore,-spokeWidth/2,0]) 
      cube([radius-toothLength-bore,spokeWidth,thickness]);
    }
    ring(hubWidth,bore,thickness);
  }
}

module escapement(radius,thickness,faceAngle,armAngle,armWidth,numberTeeth,toothSpan,hubWidth,bore)
// creates a Graham escapement
// radius is the radius of the wheel, not of the escapement
// thickess is along z
// faceAngle controls how many degrees the impulse face covers seen from the hub of the escapement wheel
// armAngle controls the angle of the escapement's arms, 0= no bend
// armWidth controls the width of the arms
// numberTeeth is the number of teeth of the wheel
// toothSpan is how many teeth the escapement spans 
// hubWidth controls the width of the hub
// bore controls the size of the bore in the hub
{
  faceWidth=2*3.1415*radius*faceAngle/360; // calculate once
  escapementAngle=180/numberTeeth*toothSpan; // calculate once

  rotate(90,[0,0,1]) // rotates escapement so pendulum is along y axis
  union()
  {
    // this is the hub
    ring(hubWidth,bore,thickness);

    // this is the pair of pallets (an entry and an exit pallet)
    for ( k=[0,1]) // k=0 is the exit pallet, and k=1 is the entry pallet
    {
      // this is the arm of the pallet
      rotate(-90-armAngle+k*(180+2*armAngle),[0,0,1]) 
      translate([bore,-armWidth/2,0]) 
      cube([radius+faceWidth/2-bore,armWidth,thickness]);

      // this is the pallet itself
      intersection()
      {
       // this is the ring from which the "dead" faces are made
       // it is important that the arcs be smooth, i.e. $fn high
       // otherwise, there would be some recoil
        ring(radius+faceWidth/2,radius-faceWidth/2,thickness,180,180);

        // this is the cube which cuts the pallet where it meets the arm
        rotate(-90-armAngle+k*(2*armAngle+180),[0,0,1])
        translate([radius,0,0])
        rotate(180-180*k,[0,0,1]) 
        translate([-0.5*radius,0,-radius])
        cube(2*radius);

        // this is the cube which cuts the pallet at the impulse face
        translate([-2*radius*cos(escapementAngle),0,0])
        rotate(-escapementAngle,[0,0,1])
        rotate(2*escapementAngle*k,[0,0,1])
        translate([radius,0,0])
        rotate(-135,[0,0,1]) 
        translate([-0.5*radius,0,-radius])
        cube(2*radius);
      }
    }
  }
}

scale=1; // scaling factor for the whole model
radius=60*scale; // escapement wheel radius, including the teeth
thickness=5*scale; // thickness along the Z axis
bore=4.5; // radius of the bores on the wheel and the escapement

rimWidth=9*scale; // width of the escapement wheel's rim
numberSpokes=7; // number of spokes in the escapement wheel
spokeWidth=6*scale; // width of the escapement wheel's spokes

numberTeeth=15;  // number of teeth in the escapement wheel
toothLength=20*scale; // length of the tooth along longest face and to inner radius of the wheel (the tip of the tooth is what counts, not the base)
toothLean=20; // how much the tooth leans over, clockwise, in degrees
toothSharpness=30; //the angle between the two side of each tooth

toothSpan=3.5; // how many teeth the escapement spans
faceAngle=6; // how many degrees the impulse face covers seen from the hub of the escapement wheel
armAngle=22; // angle of the escapement's arms
armWidth=5*scale; //width of the escapement's arms
hubWidth=12*scale; //width of the escapement's hub

projection()escapementWheel(radius,rimWidth,thickness,numberTeeth,toothLength,toothLean,toothSharpness,numberSpokes,spokeWidth,hubWidth,bore);

projection()translate([0,2*radius*cos(180/numberTeeth*toothSpan),0])
escapement(radius,thickness,faceAngle,armAngle,armWidth,numberTeeth,toothSpan,hubWidth,bore);