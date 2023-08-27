using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;


public class Abilities : MonoBehaviour
{
    [Header("Ability 1")]
    public Image abilityImage1;
    public Text abilityText1;
    public KeyCode ability1Key;
    public float ability1Cooldown = 4;

    public Canvas ability1Canvas;
    public Image ability1Fire;

    [Header("Ability 2")]
    public Image abilityImage2;
    public Text abilityText2;
    public KeyCode ability2Key;
    public float ability2Cooldown = 6;

    public Canvas ability2Canvas;
    public Image ability2Skillshot;

    [Header("Ability 3")]
    public Image abilityImage3;
    public Text abilityText3;
    public KeyCode ability3Key;
    public float ability3Cooldown = 8;

    public Canvas ability3Canvas;
    public Image ability3Area;
    public float maxAbility3Distance = 7;

    [Header("Ability 4")]
    public Image abilityImage4;
    public Text abilityText4;
    public KeyCode ability4Key;
    public float ability4Cooldown = 10;

    public Canvas ability4Canvas;
    public Image ability4Area;

    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;
    private bool isAbility4Cooldown = false;

    private float currentAbility1Cooldown;
    private float currentAbility2Cooldown;
    private float currentAbility3Cooldown;
    private float currentAbility4Cooldown;

    private Vector3 position;
    private RaycastHit hit;
    private Ray ray;


    // Start is called before the first frame update
    void Start()
    {
        abilityImage1.fillAmount = 0;
        abilityImage2.fillAmount = 0;
        abilityImage3.fillAmount = 0;
        abilityImage4.fillAmount = 0;

        abilityText1.text = "";
        abilityText2.text = "";
        abilityText3.text = "";
        abilityText4.text = "";

        ability1Fire.enabled = false;
        ability2Skillshot.enabled = false;
        ability3Area.enabled = false;
        ability4Area.enabled = false;

        ability1Canvas.enabled = false;
        ability2Canvas.enabled = false;
        ability3Canvas.enabled = false;
        ability4Canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Ability1Input();
        Ability2Input();
        Ability3Input();
        Ability4Input();

        AbilityCooldown(ref currentAbility1Cooldown, ability1Cooldown, ref isAbility1Cooldown, abilityImage1, abilityText1);
        AbilityCooldown(ref currentAbility2Cooldown, ability2Cooldown, ref isAbility2Cooldown, abilityImage2, abilityText2);
        AbilityCooldown(ref currentAbility3Cooldown, ability3Cooldown, ref isAbility3Cooldown, abilityImage3, abilityText3);
        AbilityCooldown(ref currentAbility4Cooldown, ability4Cooldown, ref isAbility4Cooldown, abilityImage4, abilityText4);

        Ability1Canvas();
        Ability2Canvas();
        Ability3Canvas();
        Ability4Canvas();

    }

    private void Ability1Canvas()
    {
        if (ability1Fire.enabled)
        {
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

            Quaternion ab1Canvas = Quaternion.LookRotation(position - transform.position);
            ab1Canvas.eulerAngles = new Vector3(0, ab1Canvas.eulerAngles.y, ab1Canvas.eulerAngles.z);

            ability1Canvas.transform.rotation = Quaternion.Lerp(ab1Canvas, ability1Canvas.transform.rotation, 0);       
        }
    }

    private void Ability2Canvas()
    {
        if (ability2Skillshot.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

            Quaternion ab2Canvas = Quaternion.LookRotation(position - transform.position);
            ab2Canvas.eulerAngles = new Vector3(0, ab2Canvas.eulerAngles.y, ab2Canvas.eulerAngles.z);

            ability2Canvas.transform.rotation = Quaternion.Lerp(ab2Canvas, ability2Canvas.transform.rotation, 0);
        }
    }

    private void Ability3Canvas()
    {

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))

        {
            if (hit.collider.gameObject != this.gameObject)
            {
                position = hit.point;
            }
        }

        var hitPosDir = (hit.point - transform.position).normalized;
        float distance = Vector3.Distance(hit.point, transform.position);
        distance = Mathf.Min(distance, maxAbility3Distance);

        var newHitPos = transform.position + hitPosDir * distance;
        ability3Canvas.transform.position = (newHitPos);

    }

    private void Ability4Canvas()
    {
        if (ability2Skillshot.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }

            Quaternion ab4Canvas = Quaternion.LookRotation(position - transform.position);
            ab4Canvas.eulerAngles = new Vector3(0, ab4Canvas.eulerAngles.y, ab4Canvas.eulerAngles.z);

            ability4Canvas.transform.rotation = Quaternion.Lerp(ab4Canvas, ability4Canvas.transform.rotation, 0);
        }

    }


    private void Ability1Input()
    {
        if (Input.GetKeyDown(ability1Key) && !isAbility1Cooldown)
        {
            ability1Canvas.enabled = true;
            ability1Fire.enabled = true;

            ability2Canvas.enabled = false;
            ability2Skillshot.enabled = false;
            ability3Canvas.enabled = false;
            ability3Area.enabled = false;
            ability4Area.enabled = false;
            ability4Canvas.enabled = false;

            Cursor.visible = true;
        }

        if (ability1Fire.enabled && Input.GetMouseButtonDown(0))
        {
            isAbility1Cooldown = true;
            currentAbility1Cooldown = ability1Cooldown;

            ability1Canvas.enabled = false;
            ability1Fire.enabled = false;
        }

    }

    private void Ability2Input()
    {
        if (Input.GetKeyDown(ability2Key) && !isAbility2Cooldown)
        {
            ability2Canvas.enabled = true;
            ability2Skillshot.enabled = true;

            ability1Canvas.enabled = false;
            ability1Fire.enabled = false;
            ability3Canvas.enabled = false;
            ability3Area.enabled = false;
            ability4Area.enabled = false;
            ability4Canvas.enabled = false;

            Cursor.visible = true;
        }

        if (ability2Skillshot.enabled && Input.GetMouseButtonDown(0))
        {
            isAbility2Cooldown = true;
            currentAbility2Cooldown = ability2Cooldown;

            ability2Canvas.enabled = false;
            ability2Skillshot.enabled = false;
        }
    }

    private void Ability3Input()
    {
        if (Input.GetKeyDown(ability3Key) && !isAbility3Cooldown)
        {
            ability3Canvas.enabled = true;
            ability3Area.enabled = true;

            ability1Canvas.enabled = false;
            ability1Fire.enabled = false;
            ability2Canvas.enabled = false;
            ability2Skillshot.enabled = false;
            ability4Area.enabled = false;
            ability4Canvas.enabled = false;

            Cursor.visible = false;     
        }
        if (ability3Canvas.enabled && Input.GetMouseButtonDown(0))
        {
            isAbility3Cooldown = true;
            currentAbility3Cooldown = ability3Cooldown;

            ability3Canvas.enabled = false; ;
            ability3Area.enabled = false;

            Cursor.visible = true;
        }
    }

    private void Ability4Input()
    {
        if (Input.GetKeyDown(ability4Key) && !isAbility4Cooldown)
        {
            ability4Canvas.enabled = true;
            ability4Area.enabled = true;

            ability2Canvas.enabled = false;
            ability2Skillshot.enabled = false;
            ability3Canvas.enabled = false;
            ability3Area.enabled = false;
            ability1Fire.enabled = false;
            ability1Canvas.enabled = false;

            Cursor.visible = false;
        }

        if (ability4Area.enabled)
        {
            isAbility4Cooldown = true;
            currentAbility4Cooldown = ability4Cooldown;

            ability4Canvas.enabled = false;
            ability4Area.enabled = false;

            Cursor.visible = true;
        }
    }

    private void AbilityCooldown(ref float currentCooldown, float maxCooldown, ref bool isCooldown, Image skillImage, Text skillText)
    {
        if(isCooldown)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0f)
            {
                isCooldown = false;
                currentCooldown = 0f;

                if(skillImage != null)
                {
                    skillImage.fillAmount = 0f;
                }
                if(skillText != null)
                {
                    skillText.text = "";
                }
            }
            else
            {
                if(skillImage != null)
                {
                    skillImage.fillAmount = currentCooldown / maxCooldown;
                }
                if(skillText != null)
                {
                    skillText.text = Mathf.Ceil(currentCooldown).ToString();
                }
            }
        }
    }
}
