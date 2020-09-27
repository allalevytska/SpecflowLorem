Feature: Generate Lorem Ipsum

Background:
Given the Lipsum page is open
@mytag
Scenario: Generate Lorem Ipsum amount of types
	When I click on <type>
	And enter the <amount>
	And press "Generate Lorem Ipsum"
	Then the result contains <amount> <type>
	Examples: 
	| type       | amount |
	| words      | 10     |
	| paras      | 5      |
	| words      | 0      |

Scenario: Generate Lorem Ipsum
When I change language to Українська
And click "Згенерувати Lorem Ipsum" 10 times
Then the avarage number of paragraphs containing the word “lorem” is more than 1 (>1)