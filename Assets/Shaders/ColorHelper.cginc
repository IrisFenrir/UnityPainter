fixed3 HSV2RGB(fixed3 hsv)
{
    fixed R, G, B;
    if (hsv.y == 0)
    {
        R = G = B = hsv.z;
    }
    else
    {
        hsv.x = hsv.x / 60.0;
        int i = (int)hsv.x;
        fixed f = hsv.x - (fixed)i;
        fixed a = hsv.z * (1 - hsv.y);
        fixed b = hsv.z * (1 - hsv.y * f);
        fixed c = hsv.z * (1 - hsv.y * (1 - f));
        switch(i)
        {
            case 0:
                R = hsv.z; G = c; B = a;
                break;
            case 1: R = b; G = hsv.z; B = a; 
                break;
            case 2: R = a; G = hsv.z; B = c; 
                break;
            case 3: R = a; G = b; B = hsv.z; 
                break;
            case 4: R = c; G = a; B = hsv.z; 
                break;
            default: R = hsv.z; G = a; B = b; 
                break;
        }
    }
    return fixed3(R, G, B);
}